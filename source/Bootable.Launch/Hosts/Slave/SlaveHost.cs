using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.Slave
{
    internal class SlaveHost : IHost
    {
        private SlaveHostSettings _settings;

        private SerialPort _port;
        private Thread _powerStateThread;

        public event EventHandler ShutDown;

        public SlaveHost(SlaveHostSettings settings)
        {
            _settings = settings;
        }

        public Task StartAsync()
        {
            _port = new SerialPort(_settings.PortName);
            _port.Open();

            Send("");
            // Set to digital input
            Send("CH1.SETMODE(2)");

            if (IsOn())
            {
                TogglePowerSwitch();
                WaitPowerState(false);
                // Small pause for discharge
                Thread.Sleep(1000);
            }

            TogglePowerSwitch();
            // Give PC some time to turn on, else we will detect it as off right away.
            WaitPowerState(true);
            
            _powerStateThread = new Thread(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        if (!IsOn())
                        {
                            _port.Close();
                            ShutDown?.Invoke(this, EventArgs.Empty);
                            break;
                        }
                    }
                });

            _powerStateThread.Start();
            return Task.CompletedTask;
        }

        public Task KillAsync()
        {
            if (_powerStateThread != null)
            {
                _powerStateThread.Abort();
                _powerStateThread.Join();
            }

            if (IsOn())
            {
                TogglePowerSwitch();
                WaitPowerState(false);
            }

            _port.Close();

            return Task.CompletedTask;
        }

        private string WaitForPrompt()
        {
            var stringBuilder = new StringBuilder();
            char previousChar = ' ';
            char currentChar = ' ';
            while (true)
            {
                previousChar = currentChar;
                currentChar = (char)_port.ReadChar();
                stringBuilder.Append(currentChar);
                if (currentChar == ':' && previousChar == ':')
                {
                    break;
                }
            }
            // Remove ::
            stringBuilder.Length = stringBuilder.Length - 2;
            return stringBuilder.ToString();
        }

        private void TogglePowerSwitch()
        {
            Send("REL4.ON");
            Thread.Sleep(500);
            Send("REL4.OFF");
        }

        private bool IsOn()
        {
            var result = Send("CH1.GET").Split('\n');
            return result[1][0] == '1';
        }

        private string Send(string data)
        {
            // Dont use writeline, it only sends /n or /r (didnt bother to find out which, we need both)
            _port.Write(data + "\r\n");
            return WaitForPrompt();
        }

        private void WaitPowerState(bool on)
        {
            int count = 0;
            while (IsOn() == !on)
            {
                Thread.Sleep(250);
                count++;
                // 5 seconds
                if (count == 20)
                {
                    throw new Exception("Slave did not respond to power command.");
                }
            }
        }
    }
}

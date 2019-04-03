#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGNapoleonics.HexgridExampleWpf {
    public sealed partial class CommandSlider : Slider, ICommandSource {
        public CommandSlider() : base() { ; }

        #region Dependency Property (static) Backing Stores
        // Using DependencyProperties as the backing store enables animation, styling, binding, etc...
        /// <summary>TODO</summary>
        public static readonly DependencyProperty CommandProperty =
                DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandSlider),
                      new UIPropertyMetadata(null,new PropertyChangedCallback(CommandChanged)));
        /// <summary>TODO</summary>
        public static readonly DependencyProperty CommandParameterProperty =
                DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandSlider), 
                      new UIPropertyMetadata(null));
        /// <summary>TODO</summary>
        public static readonly DependencyProperty CommandTargetProperty =
                DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(CommandSlider),
                      new UIPropertyMetadata(null));
        #endregion

        public ICommand      Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public object        CommandParameter {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public IInputElement CommandTarget {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        #region Event Handlers
        /// <summary>Command dependency property change callback.</summary>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        =>  ((CommandSlider)d).HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);

        /// <inheritdoc/>
        /// <remarks>
        /// If Command is defined, moving the slider will invoke the command; 
        /// Otherwise, the slider will behave normally. 
        /// </remarks>
        protected override void OnValueChanged(double oldValue, double newValue) {
            base.OnValueChanged(oldValue, newValue);

            if (Command != null) {
                if(Command is RoutedCommand routedCmd) {
                    routedCmd.Execute(CommandParameter, CommandTarget);
                }  else {
                    Command.Execute(CommandParameter);
                }
            }
        }
        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);

            var command = Command;
            var parameter = CommandParameter;
            var target = CommandTarget;

            if(command is RoutedCommand routedCmd && routedCmd.CanExecute(parameter, target)) {
                routedCmd.Execute(parameter, target);
            } else if(command != null && command.CanExecute(parameter)) {
                command.Execute(parameter);
            }
        }
        /// <summary>TODO</summary>
        private void CanExecuteChanged(object sender, EventArgs e) {
            if (Command != null)    {
                if(Command is RoutedCommand routedCmd) {
                    IsEnabled = routedCmd.CanExecute(CommandParameter, CommandTarget);
                } else {
                    IsEnabled = Command.CanExecute(CommandParameter);
                }
            }
        }
        #endregion

        /// <summary>Add a new command to the Command Property. </summary>
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand) {
            if (oldCommand != null)   oldCommand.CanExecuteChanged -= CanExecuteChanged;
            if (newCommand != null)   newCommand.CanExecuteChanged += CanExecuteChanged;
        }
    }
}
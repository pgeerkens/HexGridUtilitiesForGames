#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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

namespace HexgridExampleWpf {
  public sealed partial class CommandComboBox : ComboBox, ICommandSource {
    public CommandComboBox() : base() { ; }

    #region Dependency Property (static) Backing Stores
    // Using DependencyProperties as the backing store enables animation, styling, binding, etc...
    /// <summary>TODO</summary>
    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandComboBox),
              new UIPropertyMetadata(null,new PropertyChangedCallback(CommandChanged)));
    /// <summary>TODO</summary>
    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandComboBox), 
              new UIPropertyMetadata(null));
    /// <summary>TODO</summary>
    public static readonly DependencyProperty CommandTargetProperty =
      DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(CommandComboBox),
              new UIPropertyMetadata(null));
    #endregion

    public ICommand      Command          {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }
    public object        CommandParameter {
      get { return (object)GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }
    public IInputElement CommandTarget    {
      get { return (IInputElement)GetValue(CommandTargetProperty); }
      set { SetValue(CommandTargetProperty, value); }
    }

    #region Event Handlers
    /// <summary>Command dependency property change callback.</summary>
    private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var cs = (CommandComboBox)d;
      cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// If Command is defined, moving the slider will invoke the command; 
    /// Otherwise, the slider will behave normally. 
    /// </remarks>
    protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
      base.OnSelectionChanged(e);

      if (this.Command != null) {
        RoutedCommand command = Command as RoutedCommand;

        if (command != null)
          command.Execute(CommandParameter, CommandTarget);
        else
          ((ICommand)Command).Execute(CommandParameter);
      }
    }
    /// <inheritdoc/>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
      base.OnMouseLeftButtonUp(e);

      var command = Command;
      var parameter = CommandParameter;
      var target = CommandTarget;

      var routedCmd = command as RoutedCommand;
      if (routedCmd != null && routedCmd.CanExecute(parameter, target))
      {
          routedCmd.Execute(parameter, target);
      }
      else if (command != null && command.CanExecute(parameter))
      {
          command.Execute(parameter);
      }
    }
    /// <summary>TODO</summary>
    private            void CanExecuteChanged(object sender, EventArgs e) {
      if (this.Command != null)    {
        var routed = this.Command as RoutedCommand;
        if (routed == null)       // If not a RoutedCommand. 
          this.IsEnabled = Command.CanExecute(CommandParameter);
        else                      // Else a RoutedCommand. 
          this.IsEnabled = routed.CanExecute(CommandParameter, CommandTarget);
      }
    }
    #endregion

    /// <summary>Add a new command to the Command Property. </summary>
    private void HookUpCommand(ICommand oldCommand, ICommand newCommand) {
      if (oldCommand != null)   oldCommand.CanExecuteChanged -= this.CanExecuteChanged;
      if (newCommand != null)   newCommand.CanExecuteChanged += this.CanExecuteChanged;
    }
  }
}

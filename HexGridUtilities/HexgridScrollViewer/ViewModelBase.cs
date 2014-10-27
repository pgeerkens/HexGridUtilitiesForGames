#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

using PGNapoleonics.HexUtilities.Common;

namespace PGNapoleonics.HexgridScrollViewer {
  /// <summary>TODO</summary>
  public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable {
    /// <summary>TODO</summary>
    protected ViewModelBase(string displayName) { DisplayName = displayName; }

    /// <summary>TODO</summary>
    public            string DisplayName                { get; private set; }
    /// <summary>TODO</summary>
    protected virtual bool   ThrowOnInvalidPropertyName { get { return true; } }

    /// <summary>Raised when a property on this object has a new value.</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Raises this object's PropertyChanged event.</summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName) {
      this.VerifyPropertyName(propertyName);
      this.PropertyChanged.Raise(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Verify that propertyName exists as public instance property on this object.</summary>
    [Conditional("DEBUG"), DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName) {
      if (TypeDescriptor.GetProperties(this)[propertyName] == null) {
        string msg = "Invalid property name: " + propertyName;
        if (this.ThrowOnInvalidPropertyName)       throw new ArgumentOutOfRangeException("propertyName",msg);

        Debug.Fail(msg);
      }
    }

    #region IDisposable implementation with Finalizeer
    bool _isDisposed = false;
    /// <inheritdoc/>
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    /// <summary>Anchors the Dispose chain for sub-classes.</summary>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
        }
        _isDisposed = true;
      }
    }
    /// <summary>Finalize this instance.</summary>
    ~ViewModelBase() { Dispose(false); }
    #endregion
  }

    /// <summary>TODO</summary>
  public class CommandViewModel : ViewModelBase { 
    /// <summary>TODO</summary>
    public CommandViewModel(string displayName, ICommand command) : base(displayName) { 
      if (command == null) throw new ArgumentNullException("command"); 
      this.Command = command; 
    } 
    /// <summary>TODO</summary>
    public ICommand Command { get; private set; } 
  }

  /// <summary>TODO</summary>
  public class RelayCommand : ICommand { 
    /// <summary>TODO</summary>
    public RelayCommand(Action<object> execute) : this(execute, (o) => true) { } 
    /// <summary>TODO</summary>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute) { 
      if (execute == null) throw new ArgumentNullException("execute");
      if (canExecute == null) throw new ArgumentNullException("canExecute");

      _execute    = execute; 
      _canExecute = canExecute;
    } 
  
    /// <summary>TODO</summary>
    [DebuggerStepThrough] 
    public bool CanExecute(object parameter) { return  _canExecute(parameter); } 

    /// <summary>TODO</summary>
    public event EventHandler CanExecuteChanged { 
      add    { CommandManager.RequerySuggested += value; } 
      remove { CommandManager.RequerySuggested -= value; } 
    }
  
    /// <summary>TODO</summary>
    public void Execute(object parameter) { _execute(parameter); }

    readonly Action<object>    _execute; 
    readonly Predicate<object> _canExecute; 
  }

  ///// <summary>TODO</summary>
  //public class RelayCommand<T> : ICommand { 
  //  /// <summary>TODO</summary>
  //  public RelayCommand(Action<T> execute) : this(execute, (o) => true) { } 
  //  /// <summary>TODO</summary>
  //  public RelayCommand(Action<T> execute, Predicate<object> canExecute) { 
  //    if (execute == null) throw new ArgumentNullException("execute");
  //    if (canExecute == null) throw new ArgumentNullException("canExecute");

  //    _execute    = execute; 
  //    _canExecute = canExecute;
  //  } 
  
  //  /// <summary>TODO</summary>
  //  [DebuggerStepThrough] 
  //  public bool CanExecute(object parameter) { return  _canExecute(parameter); } 

  //  /// <summary>TODO</summary>
  //  public event EventHandler CanExecuteChanged { 
  //    add    { CommandManager.RequerySuggested += value; } 
  //    remove { CommandManager.RequerySuggested -= value; } 
  //  }
  
  //  /// <summary>TODO</summary>
  //  public void Execute(T parameter) { _execute(parameter); }

  //  readonly Action<T>    _execute; 
  //  readonly Predicate<object> _canExecute; 
  //}

  /// <summary>TODO</summary>
  public abstract class WorkspaceViewModel : ViewModelBase {
    /// <summary>TODO</summary>
    protected WorkspaceViewModel() : this ("WorkspaceViewModel_None") { ; }
    /// <summary>TODO</summary>
    protected WorkspaceViewModel(string displayName) : base (displayName) {
      _closeCommand = new RelayCommand(param => this.OnRequestClose());
    }

    /// <summary>Returns the command to remove this workspace from the user interface.</summary>
    public virtual ICommand CloseCommand { get { return _closeCommand; } } ICommand _closeCommand;

    /// <summary>Raised when this workspace should be removed from the UI.</summary>
    public event EventHandler RequestClose;

    void OnRequestClose()  { RequestClose.Raise(this,EventArgs.Empty); }
  }
}

using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AutoLedgeBook.Models;

/// <summary>
///     Базовый класс модели.
/// </summary>
public abstract class Model : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = (_,__) => { };

    private Queue<string> _suspendedMemberNames = new();
    private bool _propertyChangedSuspended = false;

    /// <summary>
    ///     Вызвать событие <see cref="PropertyChanged"/>
    /// </summary>
    /// <param name="callerName">Наименование члена, вызвавшего данный метод</param>
    protected void InvokePropertyChanged([CallerMemberName] string callerName = "")
    {
        if (!_propertyChangedSuspended)
            PropertyChanged!(this, new(callerName));

        if (!_suspendedMemberNames.Contains(callerName))
            _suspendedMemberNames.Enqueue(callerName);
    }

    /// <summary>
    ///     Изменить значение свойства и вызвать <see cref="InvokePropertyChanged(string)"/> если новое значение отличается от предыдущего.
    /// </summary>
    /// <param name="field">Ссылка на свойство которое необходимо изменить</param>
    /// <param name="value">Значение</param>
    /// <param name="comparer">Экземпляр для сравнение двух значений</param>
    /// <param name="callerName">Наименование члена, вызвавшего данный метод</param>
    /// <remarks>
    ///     Если значение параметра <paramref name="comparer"/> будет <b>null</b>. Метод подставит по умолчанию <see cref="EqualityComparer{T}.Default"/>
    /// </remarks>
    protected void ChangeProperty<TValue>(ref TValue field, TValue value, IEqualityComparer<TValue>? comparer = null, [CallerMemberName] string callerName = "")
    {
        comparer = comparer ?? EqualityComparer<TValue>.Default;

        if (!comparer.Equals(field, value))
        {
            field = value;
            InvokePropertyChanged(callerName);
        }
    }

    /// <summary>
    ///     Изменить свойство или поле в объекте.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="obj">Объект, поле или свойство которого необходимо изменить</param>
    /// <param name="propertyExpr">Выражение выбора члена объекта</param>
    /// <param name="newObjVal">Новое значение</param>
    /// <param name="comparer">Экземпляр сравнителя объектов</param>
    /// <param name="callerName">Наименование члена вызвавшего данный метод</param>
    /// <exception cref="InvalidProgramException"></exception>
    protected void ChangeProperty<TObject, TValue>(TObject obj, Expression<Func<TObject, TValue>> propertyExpr, TValue newObjVal, IEqualityComparer<TValue>? comparer = null, [CallerMemberName] string callerName = "")
    {
        comparer = comparer ?? EqualityComparer<TValue>.Default;
        
        MemberExpression memberAccessExpression = (MemberExpression)propertyExpr.Body;
        Func<TValue> getMemberValue;
        Action<TValue> setMemberValue;

        if (memberAccessExpression.Member is FieldInfo fieldInfo)
        {
            getMemberValue = () => (TValue)fieldInfo.GetValue(obj)!;
            setMemberValue = (TValue val) => fieldInfo.SetValue(obj, val);
        }
        else if (memberAccessExpression.Member is PropertyInfo propertyInfo)
        {
            getMemberValue = () => (TValue)propertyInfo.GetValue(obj)!;
            setMemberValue = (TValue val) => propertyInfo.SetValue(obj, val);
        }
        else
        {
            throw new InvalidProgramException();
        }

        TValue oldValue = getMemberValue();
        if (!comparer.Equals(oldValue, newObjVal))
        {
            setMemberValue(newObjVal);
            InvokePropertyChanged(callerName);
        }
    }


    /// <summary>
    ///     Приостановить вызов <see cref="PropertyChanged"/>
    /// </summary>
    protected void SuspendCallPropertyChanged() => _propertyChangedSuspended = true;

    /// <summary>
    ///     Продолжить вызов <see cref="PropertyChanged"/>
    /// </summary>
    /// <param name="invokeChanges">Флаг, означяющий, необходимо ли обновить все свойства, которые были вызваны, пока вызов был приостановлен</param>
    protected void ReleaseCallPropertyChanged(bool invokeChanges = true)
    {
        if (!invokeChanges)
        {
            _propertyChangedSuspended = false;
            return;
        }

        while (_suspendedMemberNames.Count > 0)
        {
            InvokePropertyChanged(_suspendedMemberNames.Dequeue());
        }
    }
}

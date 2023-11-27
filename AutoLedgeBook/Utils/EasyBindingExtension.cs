using System;
using System.Linq.Expressions;
using System.Windows.Forms;

using AutoLedgeBook.ViewModels;

namespace AutoLedgeBook.Utils;

public static class EasyBindingExtension
{
    /// <summary>
    ///     Привязать контрол к модели-представлению.
    /// </summary>
    /// <typeparam name="TControl">Тип контрола</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="control">Контрол, к которому происходит привязка</param>
    /// <param name="selectControlProperty">Выражение выбора свойства контрла</param>
    /// <param name="viewModel">Модель-представление</param>
    /// <param name="selectViewModelProperty">Выбор свойства модели-представления к которому будет происходить привязка</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static Binding Bind<TControl, TViewModel>(this TControl control,
                                                  Expression<Func<TControl, object?>> selectControlProperty,
                                                  TViewModel viewModel,
                                                  Expression<Func<TViewModel, object?>> selectViewModelProperty) where TControl : Control 
                                                                                                                where TViewModel : ViewModel
    {
        if (selectControlProperty is null)
            throw new ArgumentNullException(nameof(selectControlProperty));
        
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        if (selectViewModelProperty is null)
            throw new ArgumentNullException(nameof(selectViewModelProperty));
                   

        MemberExpression controlMemberExpression;
        if (selectControlProperty.Body is UnaryExpression un)
        {
            controlMemberExpression = (MemberExpression)un.Operand;
        }
        else
        {
            controlMemberExpression = (MemberExpression)selectControlProperty.Body;
        }

        string controlMemberName = controlMemberExpression.Member.Name;
        
        MemberExpression viewModelMemberExpression;
        if (selectViewModelProperty.Body is UnaryExpression unary)
        {
            viewModelMemberExpression = (MemberExpression)unary.Operand;
        }
        else
        {
            viewModelMemberExpression = (MemberExpression)selectViewModelProperty.Body;
        }

        string viewModelMemberName = viewModelMemberExpression.Member.Name;

        return control.DataBindings.Add(controlMemberName, viewModel, viewModelMemberName);
    }
}

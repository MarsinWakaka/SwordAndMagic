using System;

namespace Utility
{
    public class BindableProperty<T> 
    {
        public BindableProperty(T value) => Value = value;
        public BindableProperty() => Value = default;
        
        public Action<T> OnValueChanged;
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }
}
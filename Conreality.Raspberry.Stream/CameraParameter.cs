using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Inspectron.CovidTest.Raspberry
{
    /// <summary>
    /// Standard implementation of ICameraParameter interface
    /// </summary>
    public class CameraParameter : ICameraParameter, INotifyPropertyChanged
    {
        public CameraParameter(string name, Func<object> get, Action<object> set,
            IEnumerable<object> values = null)
        {
            Name = name;
            this.setter = set;
            this.getter = get;
            Values = new ReadOnlyCollection<object>(values != null ?
                values.ToList() : new List<object>());
        }

        public string Name { get; private set; }

        public object Value
        {
            get
            {
                return getter();
            }
            set
            {
                if (setter == null)
                    throw new NotSupportedException("This is a read-only parameter");

                setter(value);

                RaisePropertyChanged("Value");
            }
        }

        public bool IsReadOnly { get { return setter == null; } }

        public ReadOnlyCollection<object> Values { get; private set; }
        

        Action<object> setter;
        Func<object> getter;

        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

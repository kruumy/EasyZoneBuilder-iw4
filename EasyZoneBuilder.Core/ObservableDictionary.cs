using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EasyZoneBuilder.Core
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public ObservableDictionary()
        {

        }

        public ObservableDictionary( IDictionary<TKey, TValue> dictionary ) : base(dictionary)
        {
        }

        public ObservableDictionary( IEqualityComparer<TKey> comparer ) : base(comparer)
        {
        }

        public ObservableDictionary( int capacity ) : base(capacity)
        {
        }

        public ObservableDictionary( IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer ) : base(dictionary, comparer)
        {
        }

        public ObservableDictionary( int capacity, IEqualityComparer<TKey> comparer ) : base(capacity, comparer)
        {
        }

        protected ObservableDictionary( SerializationInfo info, StreamingContext context ) : base(info, context)
        {
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;



        protected void RaisePropertyChanged( string propName )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        protected void RaiseCollectionChanged( NotifyCollectionChangedAction action )
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
            RaisePropertyChanged(nameof(Keys));
            RaisePropertyChanged(nameof(Values));
            RaisePropertyChanged(nameof(Count));
        }

        public new void Add( TKey key, TValue val )
        {
            base.Add(key, val);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
        }

        public void AddRange( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
        {
            foreach ( KeyValuePair<TKey, TValue> item in pairs )
            {
                base.Add(item.Key, item.Value);
            }
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
        }

        public new void Clear()
        {
            base.Clear();
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public new bool Remove( TKey key )
        {
            if ( base.Remove(key) )
            {
                RaiseCollectionChanged(NotifyCollectionChangedAction.Remove);
                return true;
            }
            else
            {
                return false;
            }
        }

        public new TValue this[ TKey key ]
        {
            get => base[ key ];
            set
            {
                base[ key ] = value;
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
            }
        }

        public void SetIndexerRange( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
        {
            foreach ( KeyValuePair<TKey, TValue> item in pairs )
            {
                base[ item.Key ] = item.Value;
            }
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add);
        }
    }
}

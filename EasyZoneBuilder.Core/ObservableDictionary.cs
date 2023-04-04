using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace EasyZoneBuilder.Core
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public new TValue this[ TKey key ]
        {
            get => base[ key ];
            set
            {
                base[ key ] = value;
                RaiseCollectionAdd(key);
            }
        }

        public new void Add( TKey key, TValue val )
        {
            base.Add(key, val);
            RaiseCollectionAdd(key);
        }

        public void AddRange( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
        {
            foreach ( KeyValuePair<TKey, TValue> item in pairs )
            {
                base.Add(item.Key, item.Value);
            }
            RaiseCollectionAdd(pairs.ToList());
        }

        public new void Clear()
        {
            base.Clear();
            RaiseCollectionReset();
        }

        public new bool Remove( TKey key )
        {
            TValue val = this[ key ];
            if ( base.Remove(key) )
            {
                RaiseCollectionRemove(new KeyValuePair<TKey, TValue>(key, val));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetIndexerRange( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
        {
            foreach ( KeyValuePair<TKey, TValue> item in pairs )
            {
                base[ item.Key ] = item.Value;
            }
            RaiseCollectionAdd(pairs.ToList());
        }


        protected void RaiseCollectionRemove( KeyValuePair<TKey, TValue> item )
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); // TODO find out Remove not working
            RaiseCollectionChangedBase();
        }
        protected void RaiseCollectionAdd( IList<KeyValuePair<TKey, TValue>> items )
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items));
            RaiseCollectionChangedBase();
        }

        protected void RaiseCollectionAdd( KeyValuePair<TKey, TValue> item )
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            RaiseCollectionChangedBase();
        }

        protected void RaiseCollectionAdd( TKey key )
        {
            RaiseCollectionAdd(new KeyValuePair<TKey, TValue>(key, this[ key ]));
        }

        protected void RaiseCollectionAdd( IEnumerable<TKey> keys )
        {
            IList<KeyValuePair<TKey, TValue>> items = new List<KeyValuePair<TKey, TValue>>();
            foreach ( TKey key in keys )
            {
                items.Add(new KeyValuePair<TKey, TValue>(key, this[ key ]));
            }
            RaiseCollectionAdd(items);
        }

        protected void RaiseCollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            RaiseCollectionChangedBase();
        }

        protected void RaisePropertyChanged( string propName )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void RaiseCollectionChangedBase()
        {
            RaisePropertyChanged(nameof(Keys));
            RaisePropertyChanged(nameof(Values));
            RaisePropertyChanged(nameof(Count));
        }
    }
}

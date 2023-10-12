using System;
using System.Collections.Generic;

namespace TargetPathology.UI.Messaging
{
	public class SimpleMessenger
	{
		private readonly Dictionary<Type, List<Action<object>>> _subscribers = new();

		public void Subscribe<TMessage>(Action<TMessage> action)
		{
			var messageType = typeof(TMessage);

			if (_subscribers.ContainsKey(messageType) == false)
			{
				_subscribers[messageType] = new List<Action<object>>();
			}

			_subscribers[messageType].Add(x => action((TMessage)x));
		}

		public void Send<TMessage>(TMessage message)
		{
			var messageType = typeof(TMessage);

			if (_subscribers.ContainsKey(messageType) == false) return;

			foreach (var subscriber in _subscribers[messageType])
			{
				subscriber(message);
			}
		}
	}

}

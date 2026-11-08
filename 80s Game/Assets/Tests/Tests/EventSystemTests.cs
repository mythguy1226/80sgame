using NUnit.Framework;
using GameplayEvents;

public class EventySystemTests
{
    private class TestGameplayEvent : IGameplayMessage
    {
        public int Value { get; set; }
    }
    private class TestGameplayListener : IGameplayListener<TestGameplayEvent>
    {
        private int _value;
        public int TestValue { get { return _value; } }
        public void OnGameplayEvent(TestGameplayEvent e)
        {
            _value = e.Value;
        }
    }

    // A Test behaves as an ordinary method
    [Test]
    public void RegisterListenerTest()
    {
        // Arrange
        GameEventBus.Reset();
        TestGameplayListener listener = new TestGameplayListener();

        // Act
        GameEventBus.Register<TestGameplayEvent>(listener);

        // Assert
        Assert.AreEqual(1, GameEventBus.ListenerCount<TestGameplayEvent>());
    }

    [Test]
    public void UnregisterListenerTest() {
        // Arrange
        GameEventBus.Reset();
        TestGameplayListener listener = new TestGameplayListener();

        // Act
        GameEventBus.Register<TestGameplayEvent>(listener);
        GameEventBus.Unregister<TestGameplayEvent>(listener);

        // Assert
        Assert.AreEqual(0, GameEventBus.ListenerCount<TestGameplayEvent>());
    }

    [Test]
    public void PropagateEventTest()
    {
        // Arrange
        GameEventBus.Reset();
        TestGameplayListener listener = new TestGameplayListener();
        TestGameplayEvent testEvent = new TestGameplayEvent();
        testEvent.Value = 10;

        // Act
        GameEventBus.Register<TestGameplayEvent>(listener);
        GameEventBus.Propagate(testEvent);

        // Assert
        Assert.AreEqual(testEvent.Value, listener.TestValue);
    }

}
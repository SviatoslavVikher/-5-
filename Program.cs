using System;
using System.Collections.Generic;

public class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();

    private Logger() { }

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
            }
            return _instance;
        }
    }

    public void Log(string message)
    {
        Console.WriteLine($"[ЛОГ]: {message}");
    }
}

public interface ITextFormat
{
    string GetContent();
}

public class TxtFile
{
    public string ReadTxt()
    {
        return "Дані з TXT файлу";
    }
}

public class JsonFile
{
    public string ReadJson()
    {
        return "{ \"message\": \"Дані з JSON файлу\" }";
    }
}

public class XmlFile
{
    public string ReadXml()
    {
        return "<message>Дані з XML файлу</message>";
    }
}

public class TextFormatAdapter : ITextFormat
{
    private object _adaptee;

    public TextFormatAdapter(object adaptee)
    {
        _adaptee = adaptee;
    }

    public string GetContent()
    {
        return _adaptee switch
        {
            TxtFile txt => txt.ReadTxt(),
            JsonFile json => json.ReadJson(),
            XmlFile xml => xml.ReadXml(),
            _ => throw new NotSupportedException("Невідомий формат")
        };
    }
}

public interface IObserver
{
    void Update(string message);
}

public class User : IObserver
{
    public string Name { get; }

    public User(string name)
    {
        Name = name;
    }

    public void Update(string message)
    {
        Console.WriteLine($"{Name} отримав повідомлення: {message}");
    }
}

public class Chat
{
    private List<IObserver> _subscribers = new();

    public void Subscribe(IObserver observer)
    {
        if (!_subscribers.Contains(observer))
            _subscribers.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        if (_subscribers.Contains(observer))
            _subscribers.Remove(observer);
    }

    public void SendMessage(string message)
    {
        Console.WriteLine($"Повідомлення в чаті: {message}");
        NotifySubscribers(message);
    }

    private void NotifySubscribers(string message)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.Update(message);
        }
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Logger.Instance.Log("Запуск програми.");

        var txtAdapter = new TextFormatAdapter(new TxtFile());
        var jsonAdapter = new TextFormatAdapter(new JsonFile());
        var xmlAdapter = new TextFormatAdapter(new XmlFile());

        Console.WriteLine(txtAdapter.GetContent());
        Console.WriteLine(jsonAdapter.GetContent());
        Console.WriteLine(xmlAdapter.GetContent());

        Chat chat = new Chat();

        User user1 = new User("Олена");
        User user2 = new User("Іван");

        chat.Subscribe(user1);
        chat.Subscribe(user2);

        chat.SendMessage("Привіт усім у чаті!");

        chat.Unsubscribe(user2);

        chat.SendMessage("Тільки Олена побачить це повідомлення.");

        Logger.Instance.Log("Завершення роботи програми.");
    }
}
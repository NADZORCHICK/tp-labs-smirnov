using System;
using System.Collections.Generic;

namespace Lab4.Core;

// Библиотека хранит коллекцию изданий и отвечает за выдачу, возврат и поиск.
public class Library
{
    // Список базового типа: в нём могут лежать книги, журналы, электронные издания.
    private List<Publication> _publications = new List<Publication>();

    public int Count
    {
        get { return _publications.Count; }
    }

    public void Add(Publication publication)
    {
        if (publication == null)
        {
            throw new ArgumentNullException(nameof(publication));
        }
        _publications.Add(publication);
    }

    // Возвращаем КОПИЮ списка, чтобы нельзя было испортить список.
    public List<Publication> GetAll()
    {
        return new List<Publication>(_publications);
    }

    // Поиск по автору.
    public List<Publication> FindByAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Автор для поиска не может быть пустым.", nameof(author));
        }

        List<Publication> result = new List<Publication>();

        foreach (Publication p in _publications)
        {
            if (p.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(p);
            }
        }

        return result;
    }

    // Выдача издания по названию.
    public int IssueByTitle(string title)
    {
        Publication p = FindByTitle(title);
        p.Issue();
        return p.GetLoanDays(); // каждого типа свой срок
    }

    // Возврат издания по названию.
    public void ReturnByTitle(string title)
    {
        Publication p = FindByTitle(title);
        p.Return();
    }

    // Сколько изданий сейчас лежит на полках.
    public int CountAvailable()
    {
        int count = 0;
        foreach (Publication p in _publications)
        {
            if (p.IsIssued == false)
            {
                count++;
            }
        }
        return count;
    }

    private Publication FindByTitle(string title)
    {
        foreach (Publication p in _publications)
        {
            if (string.Equals(p.Title, title, StringComparison.OrdinalIgnoreCase))
            {
                return p;
            }
        }

        throw new ArgumentException("Издание не найдено: " + title);
    }
}
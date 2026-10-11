using System;
using System.IO;

// Вся логика копирования - здесь.
public class FileCopier
{
    // Проверяет введённые данные.
    // Возвращает текст ошибки, а если всё в порядке — пустую строчку.
    public string Check(string sourcePath, string destinationFolder, string newFileName)
    {
        if (File.Exists(sourcePath) == false)
        {
            return "исходный файл не найден.";
        }

        // Папку назначения создавать нельзя — она должна уже существовать.
        if (Directory.Exists(destinationFolder) == false)
        {
            return "папка назначения не существует.";
        }

        if (string.IsNullOrWhiteSpace(newFileName))
        {
            return "новое имя файла не может быть пустым.";
        }

        return "";
    }

    // Проверка, если ли файл с таким же именем.
    // Path.Combine склеивает папку и имя файла.
    public bool DestinationFileExists(string destinationFolder, string newFileName)
    {
        string destinationPath = Path.Combine(destinationFolder, newFileName);
        return File.Exists(destinationPath);
    }

    // Копирует файл. Возвращает пустую строчку, если всё прошло успешно,
    // иначе — текст ошибки.
    // overwrite == true разрешает перезаписать сущ. файл
    // (вызывается только после подтверждения от пользователя).
    public string Copy(string sourcePath, string destinationFolder, string newFileName, bool overwrite)
    {
        try
        {
            string destinationPath = Path.Combine(destinationFolder, newFileName);
            File.Copy(sourcePath, destinationPath, overwrite);
            return "";
        }
        catch (UnauthorizedAccessException)
        {
            return "нет прав доступа к файлу или папке.";
        }
        catch (IOException ex)
        {
            return "ошибка ввода-вывода: " + ex.Message;
        }
        catch (Exception ex)
        {
            return "не удалось скопировать файл: " + ex.Message;
        }
    }
}
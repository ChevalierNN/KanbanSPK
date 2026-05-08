CREATE DATABASE SPKanban
GO
USE SPKanban
GO


CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE
)
GO

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE 
)
GO

CREATE TABLE UserStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE,   
    Description NVARCHAR(255) NULL
)
GO

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Login NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Patronymic NVARCHAR(50),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    RoleID INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleID),
    DepartmentID INT FOREIGN KEY REFERENCES Departments(DepartmentID),
    StatusID INT NOT NULL FOREIGN KEY REFERENCES UserStatuses(StatusID) DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
	MistakeCount INT DEFAULT 0
)
GO

CREATE TABLE TaskStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE 
)
GO

CREATE TABLE TaskPriorities (
    PriorityID INT PRIMARY KEY IDENTITY(1,1),
    PriorityName NVARCHAR(20) NOT NULL UNIQUE 
)
GO

CREATE TABLE Tasks (
    TaskID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500),
    CreatedByUserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID), 
    AssignedToUserID INT FOREIGN KEY REFERENCES Users(UserID), 
    CurrentStatusID INT NOT NULL FOREIGN KEY REFERENCES TaskStatuses(StatusID),
    PriorityID INT NOT NULL FOREIGN KEY REFERENCES TaskPriorities(PriorityID) DEFAULT 2, 
    DueDate DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
)
GO

CREATE TABLE TaskComments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    TaskID INT NOT NULL FOREIGN KEY REFERENCES Tasks(TaskID) ON DELETE CASCADE,
    UserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    CommentText NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
)
GO


INSERT INTO Departments (DepartmentName) VALUES 
('IT Отдел'), 
('Маркетинг'), 
('Финансы')
GO

INSERT INTO Roles (RoleName) VALUES 
('Администратор'), 
('Менеджер'), 
('Исполнитель')
GO

INSERT INTO UserStatuses (StatusName, Description) VALUES
('Active', 'активен'),
('Blocked', 'заблокирован')
GO

INSERT INTO Users (Login, [Password], FirstName, LastName, Patronymic, Email, Phone, RoleID, DepartmentID, StatusID, CreatedAt) VALUES
( '1', '1', 'Александр', 'Иванов', 'Сергеевич', 'admin@company.ru', '+7 (900) 111-11-11', 1, 3, 1, GETDATE()),
( '2', '2', 'Елена', 'Петрова', 'Викторовна', 'petrova@company.ru', '+7 (900) 222-22-22', 2, 3, 1, GETDATE()),
( '3', '3', 'Дмитрий', 'Смирнов', 'Алексеевич', 'smirnov@company.ru', '+7 (900) 333-33-33', 2, 1, 1, GETDATE()),
( '4', '4', 'Игорь', 'Кузнецов', 'Анатольевич', 'kuznetsov@company.ru', '+7 (900) 444-44-44', 3, 1, 1, GETDATE()),
( '5', '5', 'Анна', 'Волкова', 'Сергеевна', 'volkova@company.ru', '+7 (900) 555-55-55', 3, 1, 1, GETDATE())
GO

INSERT INTO TaskStatuses (StatusName) VALUES 
('В работе'), 
('На проверке'), 
('Готово')
GO

INSERT INTO TaskPriorities (PriorityName) VALUES 
('Низкий'), 
('Средний'), 
('Высокий')
GO

INSERT INTO Tasks (Title, Description, CreatedByUserID, AssignedToUserID, CurrentStatusID, PriorityID, DueDate, CreatedAt, UpdatedAt)
VALUES
('Настроить резервное копирование', 'Настроить ежедневный бэкап БД на внешний диск', 2, 4, 1, 3, DATEADD(day, 2, GETDATE()), GETDATE(), GETDATE()),
('Обновить драйверы на ПК бухгалтерии', 'Обновить видеодрайверы и драйверы принтера', 2, 4, 1, 2, DATEADD(day, -1, GETDATE()), DATEADD(day, -1, GETDATE()), GETDATE()),
('Проверить сетевые кабели в кабинете 305', 'Диагностика обрыва сети в переговорной', 2, 4, 1, 3, GETDATE(), DATEADD(hour, -3, GETDATE()), GETDATE()),
('Установить 1С на новый сервер', 'Развёртывание 1С:ERP + настройка прав доступа', 2, 4, 2, 3, DATEADD(day, -3, GETDATE()), DATEADD(day, -3, GETDATE()), DATEADD(hour, -2, GETDATE())), 
('Настроить учётную запись нового сотрудника', 'Создать учётку в AD, почту, доступ к SharePoint', 2, 5, 2, 2, DATEADD(day, -2, GETDATE()), DATEADD(day, -2, GETDATE()), DATEADD(hour, -1, GETDATE())),
('Протестировать форму авторизации', 'Проверить логин/пароль, восстановление, 2FA', 2, 5, 2, 2, DATEADD(day, 1, GETDATE()), DATEADD(day, -1, GETDATE()), GETDATE()),
('Подключить новый монитор в кабинет директора', 'Установка и калибровка Dell U2723QE', 2, 5, 3, 1, DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE()), DATEADD(day, -2, GETDATE())),
('Обновить антивирус на всех ПК', 'Развёртывание Kaspersky Endpoint 12.7', 2, 5, 3, 2, DATEADD(day, -8, GETDATE()), DATEADD(day, -8, GETDATE()), DATEADD(day, -1, GETDATE())),
('Настроить Wi-Fi в зоне отдыха', 'Установка точки доступа TP-Link Omada', 2, 4, 3, 3, DATEADD(day, -5, GETDATE()), DATEADD(day, -5, GETDATE()), DATEADD(day, -3, GETDATE()))
GO

INSERT INTO TaskComments (TaskID, UserID, CommentText, CreatedAt)
VALUES
(1, 4, 'Начал настройку. Нужны права администратора на сервере.', GETDATE()),
(1, 2, 'Права выданы. Ожидаем результат к пятнице.', DATEADD(hour, 1, GETDATE())),
(2, 4, 'Драйвера обновлены. Принтер работает стабильно.', DATEADD(day, -1, GETDATE())),
(4, 5, 'Сервер развёрнут. Ожидаю проверки от менеджера.', DATEADD(day, -2, GETDATE())),
(9, 4, 'Точка доступа установлена. Сигнал стабилен по всему этажу.', DATEADD(day, -3, GETDATE()))
GO
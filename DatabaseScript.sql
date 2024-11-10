
create database Restaurant
go
use Restaurant
go
CREATE TABLE Role (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255),
    deleteFlag BIT DEFAULT 0,
	createAt DATETIME,
	updateAt DATETIME,
	deleteAt DATETIME
);

CREATE TABLE Account (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(255) UNIQUE,
    password VARCHAR(60),
	code VARCHAR(20),
    roleID INT,
    isActive BIT,
	createAt DATETIME,
	updateAt DATETIME,
	deleteAt DATETIME,
    FOREIGN KEY (roleID) REFERENCES Role(id)
);

CREATE TABLE Token(
    id INT IDENTITY(1,1) PRIMARY KEY,
    token VARCHAR(255),
    accountId INT,
	createAt DATETIME,
    FOREIGN KEY (accountId) REFERENCES Account(id)
);

CREATE TABLE [Table] (
    id INT IDENTITY(1,1) PRIMARY KEY,
    isOrder BIT,
	forBooking Bit default 0,
    deleteFlag BIT default 0,
	createAt DATETIME,
	updateAt DATETIME,
	deleteAt DATETIME
);

CREATE TABLE Category (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255),
    deleteFlag BIT default 0,
	createAt DATETIME,
	updateAt DATETIME,
	deleteAt DATETIME
);

CREATE TABLE Menu (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255),
    detail NVARCHAR(255),
    price FLOAT,
    img NVARCHAR(255),
	quantity Int,
    cateID INT,
    deleteFlag BIT default 0,
	createAt DATETIME,
	updateAt DATETIME,
	deleteAt DATETIME,
    FOREIGN KEY (cateID) REFERENCES Category(id)
);

CREATE TABLE Ingredient(
id INT IDENTITY(1,1) PRIMARY KEY,
name NVARCHAR(255),
weight float,
price float,
createAt datetime,
updateAt datetime,	
)

CREATE TABLE Feedback(
id INT IDENTITY(1,1) PRIMARY KEY,
phone VARCHAR(255),
email VARCHAR(255),
feedback nvarchar(4000),
img nvarchar(255),
createAt datetime,
FeedbackForDate datetime,
accountId INT,
type varchar(255),
menuId int,
FOREIGN KEY (accountId) REFERENCES Account(id),
FOREIGN KEY (menuId) REFERENCES Menu(id)
)

CREATE TABLE Bill (
    id INT IDENTITY(1,1) PRIMARY KEY,
    tableID INT,
    payed BIT,
    createAt DATETIME,
	createBy INT,
    updateAt DATETIME,
	updateBy INT,
    status BIT,
    FOREIGN KEY (tableID) REFERENCES [Table](id)
);

CREATE TABLE BillInfor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    billID INT,
    menuID INT,
    quantity INT,
    price FLOAT,
	createAt DATETIME,
    updateAt DATETIME,
    FOREIGN KEY (billID) REFERENCES Bill(id),
    FOREIGN KEY (menuID) REFERENCES Menu(id)
);

CREATE TABLE Booking (
    id INT PRIMARY KEY IDENTITY(1,1),
    tableID INT,
    startDate DATETIME,
    status VARCHAR(255),
    email VARCHAR(255),
    phone VARCHAR(255),
    fullName NVARCHAR(255),
	createAt DATETIME,
    FOREIGN KEY (tableID) REFERENCES [Table](id)
);

insert into Role (name) values ('staff'),('cooker');

insert into Account (username, password, roleID, isActive, createAt) values 
('staff01@gmail.com' , '123456' , 1, 1, GETDATE());

insert into [Table] (isOrder, deleteFlag, createAt, forBooking) values
(0, 0, GETDATE(), 0),
(0, 0, GETDATE(), 0),
(0, 0, GETDATE(), 0),
(0, 0, GETDATE(), 0),
(0, 0, GETDATE(), 1),
(0, 0, GETDATE(), 1),
(0, 0, GETDATE(), 1),
(0, 0, GETDATE(), 1);

INSERT INTO Category (name, deleteFlag, createAt)
VALUES 
(N'Khai vị', 0, GETDATE()),
(N'Món chính', 0, GETDATE()),
(N'Đồ uống', 0, GETDATE()),
(N'Tráng miệng', 0, GETDATE());

INSERT INTO Menu (name, detail, price, img, cateID, deleteFlag, createAt, quantity)
VALUES
-- Mục Khai vị
(N'Súp bí đỏ', N'Súp bí đỏ kem mịn', 45000, '/assets/img/pumpkin_soup.jpg', 1, 0, GETDATE(),10),
(N'Salad Caesar', N'Salad với xà lách Romaine, sốt Caesar, và phô mai Parmesan', 50000, '/assets/img/caesar_salad.jpg', 1, 0, GETDATE(),10),
(N'Bánh mì bơ tỏi', N'Bánh mì nướng giòn với bơ và tỏi', 20000, '/assets/img/garlic_bread.jpg', 1, 0, GETDATE(),10),

-- Mục Món chính
(N'Spaghetti bò bằm', N'Mì Ý sốt bò bằm', 80000, '/assets/img/spaghetti.jpg', 2, 0, GETDATE(),10),
(N'Cơm gà xối mỡ', N'Cơm trắng với gà chiên giòn xối mỡ', 70000, '/assets/img/fried_chicken_rice.jpg', 2, 0, GETDATE(),10),
(N'Phở bò', N'Phở Việt Nam với thịt bò', 60000, '/assets/img/beef_pho.jpg', 2, 0, GETDATE(),10),

-- Mục Đồ uống
(N'Nước chanh', N'Nước chanh tươi', 20000, '/assets/img/lemonade.jpg', 3, 0, GETDATE(),10),
(N'Nước ngọt', N'Nước ngọt đóng chai', 15000, '/assets/img/soft_drink.jpg', 3, 0, GETDATE(),10),
(N'Sinh tố bơ', N'Sinh tố bơ tươi', 40000, '/assets/img/avocado_smoothie.jpg', 3, 0, GETDATE(),10),

-- Mục Tráng miệng
(N'Bánh flan', N'Bánh flan mềm mịn', 25000, '/assets/img/flan.jpg', 4, 0, GETDATE(),10),
(N'Kem dừa', N'Kem tươi vị dừa', 30000, '/assets/img/coconut_icecream.jpg', 4, 0, GETDATE(),10),
(N'Chè khúc bạch', N'Chè trái cây với khúc bạch', 35000, '/assets/img/fruit_dessert.jpg', 4, 0, GETDATE(),10);

INSERT INTO Bill (tableID, payed, createAt, createBy, updateAt, updateBy, status)
VALUES
(1, 0, GETDATE(), 1, GETDATE(), 1, 1),
(2, 0, GETDATE(), 1, GETDATE(), 1, 1),
(3, 1, GETDATE(), 1, GETDATE(), 1, 0);

INSERT INTO BillInfor (billID, menuID, quantity, price, createAt, updateAt)
VALUES
(1, 1, 2, 20000, GETDATE(), GETDATE()),
(1, 2, 1, 25000, GETDATE(), GETDATE()),
(1, 3, 1, 30000, GETDATE(), GETDATE()),

(2, 1, 1, 20000, GETDATE(), GETDATE()),
(2, 2, 2, 25000, GETDATE(), GETDATE()),
(2, 3, 1, 30000, GETDATE(), GETDATE()),

(3, 1, 1, 20000, GETDATE(), GETDATE()),
(3, 2, 1, 25000, GETDATE(), GETDATE()),
(3, 3, 2, 30000, GETDATE(), GETDATE());

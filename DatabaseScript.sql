
create database Restaurant
go
use Restaurant
go
CREATE TABLE Role (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255),
    status BIT DEFAULT 1
);

CREATE TABLE Account (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(255) UNIQUE,
    password VARCHAR(60),
    roleID INT,
    isActive BIT,
    FOREIGN KEY (roleID) REFERENCES Role(id)
);

CREATE TABLE Token(
    id INT IDENTITY(1,1) PRIMARY KEY,
    token VARCHAR(30),
    date DATETIME,
    accountId INT,
    FOREIGN KEY (accountId) REFERENCES Account(id)
);

CREATE TABLE [Table] (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255),
    isOrder BIT,
    status BIT
);

CREATE TABLE Category (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255),
    isActive BIT
);

CREATE TABLE Menu (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255),
    detail NVARCHAR(255),
    price FLOAT,
    img NVARCHAR(255),
    cateID INT,
    isSell BIT,
    FOREIGN KEY (cateID) REFERENCES Category(id)
);


CREATE TABLE Bill (
    id INT IDENTITY(1,1) PRIMARY KEY,
    tableID INT,
    payed BIT,
    createDate DATETIME,
    updateDate DATETIME,
    Status BIT,
    FOREIGN KEY (tableID) REFERENCES [Table](id)
);

CREATE TABLE BillInfor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    billID INT,
    menuID INT,
    quantity INT,
    price FLOAT,
    status BIT,
    FOREIGN KEY (billID) REFERENCES Bill(id),
    FOREIGN KEY (menuID) REFERENCES Menu(id)
);

CREATE TABLE Booking (
    id INT PRIMARY KEY IDENTITY(1,1),
    tableID INT,
    startDate DATETIME,
    Status VARCHAR(255),
    updateAt DATETIME,
    email VARCHAR(255),
    phone VARCHAR(255),
    fullName VARCHAR(255),
    FOREIGN KEY (tableID) REFERENCES [Table](id)
);

insert into Role (name) values ('staff'), ('admin');

insert into Account (username, password, roleID, isActive) values 
('staff01' , '123456' , 1, 1),
('admin01' , '123456' , 2, 1);

insert into [Table] (name, isOrder, status) values
('01', 0, 1),
('02', 0, 1),
('03', 0, 1),
('04', 0, 1),
('05', 0, 1),
('06', 0, 1),
('07', 0, 1),
('08', 0, 1);

INSERT INTO Category (name, isActive)
VALUES 
(N'Cà phê', 1),
(N'Trà', 1),
(N'Sinh tố', 1),
(N'Nước ép', 1),
(N'Đồ ăn vặt', 1),
(N'Đồ uống khác', 1);

INSERT INTO Menu (name, detail, price, img, cateID, isSell)
VALUES
-- Mục Cà phê
(N'Cà phê đen', N'Cà phê nguyên chất, đậm đà, không sữa', 20000, 'coffee_black.jpg', 1, 1),
(N'Cà phê sữa', N'Cà phê pha với sữa đặc, vị ngọt ngào', 25000, 'coffee_milk.jpg', 1, 1),
(N'Cà phê đá xay', N'Cà phê đen kết hợp với đá xay', 30000, 'coffee_frappe.jpg', 1, 1),

-- Mục Trà
(N'Trà đào cam sả', N'Trà đen, đào, cam, và sả tươi', 35000, 'peach_tea.jpg', 2, 1),
(N'Trà xanh Matcha', N'Trà xanh Nhật Bản kết hợp với sữa tươi', 45000, 'matcha_tea.jpg', 2, 1),

-- Mục Sinh tố
(N'Sinh tố xoài', N'Sinh tố xoài nguyên chất', 40000, 'mango_smoothie.jpg', 3, 1),
(N'Sinh tố dâu', N'Sinh tố dâu tươi', 40000, 'strawberry_smoothie.jpg', 3, 1),

-- Mục Nước ép
(N'Nước ép cam', N'Nước ép cam tươi', 30000, 'orange_juice.jpg', 4, 1),
(N'Nước ép dưa hấu', N'Nước ép dưa hấu tươi mát', 30000, 'watermelon_juice.jpg', 4, 1),

-- Mục Đồ ăn vặt
(N'Bánh mì nướng', N'Bánh mì nướng với bơ và mật ong', 15000, 'toast.jpg', 5, 1),
(N'Khoai tây chiên', N'Khoai tây chiên giòn', 20000, 'fries.jpg', 5, 1);

UPDATE Menu
SET img = CONCAT('/assets/img/', img);

ALTER TABLE [Table]
ADD forBooking Bit default 0;
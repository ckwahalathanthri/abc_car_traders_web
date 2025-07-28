-- =====================================================
-- ABC Car Traders Database Script
-- Generated from C# Model Files
-- MySQL Version: 8.0.36
-- =====================================================

-- Drop and create database
DROP DATABASE IF EXISTS ABCCarTradersDB;
CREATE DATABASE ABCCarTradersDB 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE ABCCarTradersDB;

-- =====================================================
-- USERS TABLE
-- =====================================================
CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(20),
    Address TEXT,
    City VARCHAR(100),
    Country VARCHAR(100),
    UserType ENUM('Admin', 'Customer') NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_email (Email),
    INDEX idx_user_type (UserType),
    INDEX idx_active (IsActive)
);

-- =====================================================
-- BRANDS TABLE
-- =====================================================
CREATE TABLE Brands (
    BrandId INT AUTO_INCREMENT PRIMARY KEY,
    BrandName VARCHAR(100) NOT NULL,
    Description TEXT,
    LogoUrl VARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_brand_name (BrandName),
    INDEX idx_active (IsActive)
);

-- =====================================================
-- CATEGORIES TABLE
-- =====================================================
CREATE TABLE Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL,
    Description TEXT,
    CategoryType ENUM('Car', 'CarPart') NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_category_name (CategoryName),
    INDEX idx_category_type (CategoryType),
    INDEX idx_active (IsActive)
);

-- =====================================================
-- CARS TABLE
-- =====================================================
CREATE TABLE Cars (
    CarId INT AUTO_INCREMENT PRIMARY KEY,
    BrandId INT NOT NULL,
    CategoryId INT NOT NULL,
    Model VARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    Color VARCHAR(50),
    Price DECIMAL(15,2) NOT NULL,
    Mileage INT,
    FuelType ENUM('Petrol', 'Diesel', 'Electric', 'Hybrid') NOT NULL,
    Transmission ENUM('Manual', 'Automatic') NOT NULL,
    EngineCapacity VARCHAR(20),
    Description TEXT,
    Features TEXT,
    ImageUrl VARCHAR(255),
    StockQuantity INT DEFAULT 1,
    IsAvailable BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (BrandId) REFERENCES Brands(BrandId) ON DELETE RESTRICT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE RESTRICT,
    
    INDEX idx_brand (BrandId),
    INDEX idx_category (CategoryId),
    INDEX idx_model (Model),
    INDEX idx_year (Year),
    INDEX idx_price (Price),
    INDEX idx_fuel_type (FuelType),
    INDEX idx_transmission (Transmission),
    INDEX idx_available (IsAvailable),
    INDEX idx_stock (StockQuantity),
    
    CONSTRAINT chk_year CHECK (Year >= 1900 AND Year <= 2030),
    CONSTRAINT chk_price CHECK (Price >= 0.01),
    CONSTRAINT chk_mileage CHECK (Mileage >= 0),
    CONSTRAINT chk_stock_quantity CHECK (StockQuantity >= 0)
);

-- =====================================================
-- CAR PARTS TABLE
-- =====================================================
CREATE TABLE CarParts (
    CarPartId INT AUTO_INCREMENT PRIMARY KEY,
    BrandId INT NOT NULL,
    CategoryId INT NOT NULL,
    PartName VARCHAR(200) NOT NULL,
    PartNumber VARCHAR(100) NOT NULL UNIQUE,
    Price DECIMAL(10,2) NOT NULL,
    Description TEXT,
    Compatibility TEXT,
    ImageUrl VARCHAR(255),
    StockQuantity INT DEFAULT 0,
    IsAvailable BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (BrandId) REFERENCES Brands(BrandId) ON DELETE RESTRICT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId) ON DELETE RESTRICT,
    
    INDEX idx_brand (BrandId),
    INDEX idx_category (CategoryId),
    INDEX idx_part_name (PartName),
    INDEX idx_part_number (PartNumber),
    INDEX idx_price (Price),
    INDEX idx_available (IsAvailable),
    INDEX idx_stock (StockQuantity),
    
    CONSTRAINT chk_part_price CHECK (Price >= 0.01),
    CONSTRAINT chk_part_stock CHECK (StockQuantity >= 0)
);

-- =====================================================
-- ORDERS TABLE
-- =====================================================
CREATE TABLE Orders (
    OrderId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    OrderNumber VARCHAR(50) NOT NULL UNIQUE,
    TotalAmount DECIMAL(15,2) NOT NULL,
    OrderStatus ENUM('Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled') DEFAULT 'Pending',
    PaymentStatus ENUM('Pending', 'Paid', 'Failed', 'Refunded') DEFAULT 'Pending',
    PaymentMethod ENUM('CreditCard', 'DebitCard', 'BankTransfer', 'Cash') NOT NULL,
    ShippingAddress TEXT NOT NULL,
    Notes TEXT,
    OrderDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE RESTRICT,
    
    INDEX idx_user (UserId),
    INDEX idx_order_number (OrderNumber),
    INDEX idx_order_status (OrderStatus),
    INDEX idx_payment_status (PaymentStatus),
    INDEX idx_payment_method (PaymentMethod),
    INDEX idx_order_date (OrderDate),
    
    CONSTRAINT chk_total_amount CHECK (TotalAmount >= 0.01)
);

-- =====================================================
-- ORDER ITEMS TABLE
-- =====================================================
CREATE TABLE OrderItems (
    OrderItemId INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    ItemType ENUM('Car', 'CarPart') NOT NULL,
    ItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(15,2) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    
    INDEX idx_order (OrderId),
    INDEX idx_item_type (ItemType),
    INDEX idx_item_id (ItemId),
    INDEX idx_item_type_id (ItemType, ItemId),
    
    CONSTRAINT chk_quantity CHECK (Quantity >= 1),
    CONSTRAINT chk_unit_price CHECK (UnitPrice >= 0.01),
    CONSTRAINT chk_total_price CHECK (TotalPrice >= 0.01)
);

-- =====================================================
-- CART TABLE
-- =====================================================
CREATE TABLE Cart (
    CartId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ItemType ENUM('Car', 'CarPart') NOT NULL,
    ItemId INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    
    UNIQUE KEY unique_user_item (UserId, ItemType, ItemId),
    INDEX idx_user (UserId),
    INDEX idx_item_type (ItemType),
    INDEX idx_item_id (ItemId),
    
    CONSTRAINT chk_cart_quantity CHECK (Quantity >= 1)
);

-- =====================================================
-- CONTACT MESSAGES TABLE
-- =====================================================
CREATE TABLE ContactMessages (
    MessageId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Subject VARCHAR(200),
    Message TEXT NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_email (Email),
    INDEX idx_is_read (IsRead),
    INDEX idx_created_at (CreatedAt)
);

-- =====================================================
-- TRIGGERS FOR AUTO-GENERATING ORDER NUMBERS
-- =====================================================
DELIMITER //

-- Auto-generate order number trigger
CREATE TRIGGER tr_Orders_GenerateOrderNumber
BEFORE INSERT ON Orders
FOR EACH ROW
BEGIN
    IF NEW.OrderNumber IS NULL OR NEW.OrderNumber = '' THEN
        SET NEW.OrderNumber = CONCAT(
            'ORD-', 
            YEAR(NOW()), 
            LPAD(MONTH(NOW()), 2, '0'), 
            '-', 
            LPAD((
                SELECT COUNT(*) + 1 
                FROM Orders 
                WHERE YEAR(OrderDate) = YEAR(NOW()) 
                AND MONTH(OrderDate) = MONTH(NOW())
            ), 6, '0')
        );
    END IF;
END //

-- Update car availability based on stock
CREATE TRIGGER tr_Cars_UpdateAvailability
BEFORE UPDATE ON Cars
FOR EACH ROW
BEGIN
    IF NEW.StockQuantity = 0 THEN
        SET NEW.IsAvailable = FALSE;
    ELSEIF NEW.StockQuantity > 0 AND OLD.StockQuantity = 0 THEN
        SET NEW.IsAvailable = TRUE;
    END IF;
END //

-- Update car part availability based on stock
CREATE TRIGGER tr_CarParts_UpdateAvailability
BEFORE UPDATE ON CarParts
FOR EACH ROW
BEGIN
    IF NEW.StockQuantity = 0 THEN
        SET NEW.IsAvailable = FALSE;
    ELSEIF NEW.StockQuantity > 0 AND OLD.StockQuantity = 0 THEN
        SET NEW.IsAvailable = TRUE;
    END IF;
END //

-- Auto-calculate order item total price
CREATE TRIGGER tr_OrderItems_CalculateTotal
BEFORE INSERT ON OrderItems
FOR EACH ROW
BEGIN
    SET NEW.TotalPrice = NEW.UnitPrice * NEW.Quantity;
END //

CREATE TRIGGER tr_OrderItems_UpdateTotal
BEFORE UPDATE ON OrderItems
FOR EACH ROW
BEGIN
    SET NEW.TotalPrice = NEW.UnitPrice * NEW.Quantity;
END //

DELIMITER ;

-- =====================================================
-- VIEWS FOR EASY QUERYING
-- =====================================================

-- Car details with brand and category information
CREATE VIEW CarDetailsView AS
SELECT 
    c.*,
    b.BrandName,
    b.LogoUrl AS BrandLogoUrl,
    cat.CategoryName,
    cat.CategoryType,
    CASE 
        WHEN c.StockQuantity = 0 THEN 'Out of Stock'
        WHEN c.StockQuantity <= 3 THEN 'Low Stock'
        ELSE 'In Stock'
    END as StockStatus,
    CASE 
        WHEN c.StockQuantity = 0 THEN 'danger'
        WHEN c.StockQuantity <= 3 THEN 'warning'
        ELSE 'success'
    END as StockStatusColor,
    CONCAT(b.BrandName, ' ', c.Model, ' (', c.Year, ')') as DisplayName
FROM Cars c
LEFT JOIN Brands b ON c.BrandId = b.BrandId
LEFT JOIN Categories cat ON c.CategoryId = cat.CategoryId;

-- Car part details with brand and category information
CREATE VIEW CarPartDetailsView AS
SELECT 
    cp.*,
    b.BrandName,
    b.LogoUrl AS BrandLogoUrl,
    cat.CategoryName,
    cat.CategoryType,
    CASE 
        WHEN cp.StockQuantity = 0 THEN 'Out of Stock'
        WHEN cp.StockQuantity <= 10 THEN 'Low Stock'
        ELSE 'In Stock'
    END as StockStatus,
    CASE 
        WHEN cp.StockQuantity = 0 THEN 'danger'
        WHEN cp.StockQuantity <= 10 THEN 'warning'
        ELSE 'success'
    END as StockStatusColor,
    CONCAT(b.BrandName, ' ', cp.PartName) as DisplayName
FROM CarParts cp
LEFT JOIN Brands b ON cp.BrandId = b.BrandId
LEFT JOIN Categories cat ON cp.CategoryId = cat.CategoryId;

-- Order summary with customer information
CREATE VIEW OrderSummaryView AS
SELECT 
    o.*,
    u.FirstName,
    u.LastName,
    u.Email,
    u.PhoneNumber,
    CONCAT(u.FirstName, ' ', u.LastName) as CustomerName,
    COUNT(oi.OrderItemId) as TotalItems,
    SUM(oi.Quantity) as TotalQuantity,
    SUM(CASE WHEN oi.ItemType = 'Car' THEN oi.Quantity ELSE 0 END) as CarCount,
    SUM(CASE WHEN oi.ItemType = 'CarPart' THEN oi.Quantity ELSE 0 END) as CarPartCount
FROM Orders o
LEFT JOIN Users u ON o.UserId = u.UserId
LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId
GROUP BY o.OrderId;

-- Cart summary with item details
CREATE VIEW CartSummaryView AS
SELECT 
    c.*,
    u.FirstName,
    u.LastName,
    u.Email,
    CONCAT(u.FirstName, ' ', u.LastName) as CustomerName,
    CASE 
        WHEN c.ItemType = 'Car' THEN 
            (SELECT CONCAT(b.BrandName, ' ', car.Model, ' (', car.Year, ')')
             FROM Cars car 
             LEFT JOIN Brands b ON car.BrandId = b.BrandId 
             WHERE car.CarId = c.ItemId)
        ELSE 
            (SELECT CONCAT(b.BrandName, ' ', cp.PartName)
             FROM CarParts cp 
             LEFT JOIN Brands b ON cp.BrandId = b.BrandId 
             WHERE cp.CarPartId = c.ItemId)
    END as ItemName,
    CASE 
        WHEN c.ItemType = 'Car' THEN 
            (SELECT car.Price FROM Cars car WHERE car.CarId = c.ItemId)
        ELSE 
            (SELECT cp.Price FROM CarParts cp WHERE cp.CarPartId = c.ItemId)
    END as UnitPrice,
    CASE 
        WHEN c.ItemType = 'Car' THEN 
            (SELECT car.Price * c.Quantity FROM Cars car WHERE car.CarId = c.ItemId)
        ELSE 
            (SELECT cp.Price * c.Quantity FROM CarParts cp WHERE cp.CarPartId = c.ItemId)
    END as TotalPrice
FROM Cart c
LEFT JOIN Users u ON c.UserId = u.UserId;

-- =====================================================
-- SAMPLE DATA INSERTION
-- =====================================================

-- Insert default admin user
INSERT INTO Users (FirstName, LastName, Email, Password, UserType, Country) VALUES
('System', 'Administrator', 'admin@abccartraders.com', 'admin123', 'Admin', 'Sri Lanka'),
('John', 'Doe', 'john.doe@email.com', 'customer123', 'Customer', 'Sri Lanka'),
('Jane', 'Smith', 'jane.smith@email.com', 'customer456', 'Customer', 'Sri Lanka');

-- Insert car brands
INSERT INTO Brands (BrandName, Description, LogoUrl) VALUES
('Toyota', 'Japanese automotive manufacturer known for reliability', '/images/brands/toyota-logo.png'),
('Honda', 'Japanese automotive manufacturer', '/images/brands/honda-logo.png'),
('BMW', 'German luxury vehicle manufacturer', '/images/brands/bmw-logo.png'),
('Mercedes-Benz', 'German luxury automotive brand', '/images/brands/mercedes-logo.png'),
('Audi', 'German luxury vehicle manufacturer', '/images/brands/audi-logo.png'),
('Nissan', 'Japanese automotive manufacturer', '/images/brands/nissan-logo.png'),
('Hyundai', 'South Korean automotive manufacturer', '/images/brands/hyundai-logo.png'),
('Kia', 'South Korean automotive manufacturer', '/images/brands/kia-logo.png');

-- Insert car categories
INSERT INTO Categories (CategoryName, Description, CategoryType) VALUES
('Sedan', 'Four-door passenger cars', 'Car'),
('SUV', 'Sport Utility Vehicles', 'Car'),
('Hatchback', 'Cars with a hatch-type rear door', 'Car'),
('Coupe', 'Two-door cars with fixed roofs', 'Car'),
('Convertible', 'Cars with removable or folding roofs', 'Car'),
('Pickup Truck', 'Light-duty trucks', 'Car');

-- Insert car parts categories
INSERT INTO Categories (CategoryName, Description, CategoryType) VALUES
('Engine Parts', 'Engine components and accessories', 'CarPart'),
('Brake System', 'Brake pads, discs, and brake components', 'CarPart'),
('Suspension', 'Shock absorbers and suspension components', 'CarPart'),
('Electrical', 'Electrical components and accessories', 'CarPart'),
('Interior', 'Interior accessories and components', 'CarPart'),
('Exterior', 'Body parts and exterior accessories', 'CarPart'),
('Filters', 'Air, oil, and fuel filters', 'CarPart'),
('Tires & Wheels', 'Tires, rims, and wheel accessories', 'CarPart');

-- Insert sample cars
INSERT INTO Cars (BrandId, CategoryId, Model, Year, Color, Price, Mileage, FuelType, Transmission, EngineCapacity, Description, Features, ImageUrl, StockQuantity) VALUES
(1, 1, 'Camry', 2023, 'White', 28000.00, 15000, 'Petrol', 'Automatic', '2.5L', 'Reliable and efficient sedan', 'Air Conditioning, Power Steering, ABS, Airbags', '/images/cars/toyota-camry-2023.jpg', 5),
(1, 2, 'RAV4', 2023, 'Black', 32000.00, 12000, 'Hybrid', 'Automatic', '2.5L', 'Compact SUV with hybrid technology', 'AWD, Hybrid Engine, Safety Features', '/images/cars/toyota-rav4-2023.jpg', 3),
(2, 1, 'Civic', 2023, 'Blue', 25000.00, 18000, 'Petrol', 'Manual', '1.5L', 'Sporty and fuel-efficient sedan', 'Turbo Engine, Sport Mode, LED Lights', '/images/cars/honda-civic-2023.jpg', 4),
(3, 4, 'M3', 2023, 'Red', 65000.00, 8000, 'Petrol', 'Automatic', '3.0L', 'High-performance luxury coupe', 'Sport Package, Premium Sound, Navigation', '/images/cars/bmw-m3-2023.jpg', 2),
(4, 1, 'C-Class', 2024, 'Silver', 45000.00, 5000, 'Petrol', 'Automatic', '2.0L', 'Luxury sedan with advanced features', 'Mercedes User Experience, Advanced Safety', '/images/cars/mercedes-c-class-2024.jpg', 3);

-- Insert sample car parts
INSERT INTO CarParts (BrandId, CategoryId, PartName, PartNumber, Price, Description, Compatibility, ImageUrl, StockQuantity) VALUES
(1, 7, 'Engine Oil Filter', 'TOY-EOF-001', 15.99, 'High-quality engine oil filter for Toyota vehicles', 'Toyota Camry 2018-2024, RAV4 2019-2024', '/images/parts/toyota-oil-filter.jpg', 50),
(1, 8, 'Front Brake Pads', 'TOY-FBP-002', 85.99, 'Premium brake pads for Toyota front wheels', 'Toyota RAV4 2019-2024', '/images/parts/toyota-brake-pads.jpg', 30),
(2, 10, 'LED Headlight Bulb', 'HON-LHB-003', 45.99, 'LED headlight bulb for Honda vehicles', 'Honda Civic 2020-2024', '/images/parts/honda-headlight.jpg', 75),
(3, 12, 'Side Mirror Assembly', 'BMW-SMA-004', 299.99, 'Electric side mirror assembly for BMW', 'BMW M3 2020-2024', '/images/parts/bmw-mirror.jpg', 10),
(4, 7, 'Air Filter', 'MBZ-AF-005', 35.99, 'High-performance air filter for Mercedes-Benz', 'Mercedes C-Class 2020-2024', '/images/parts/mercedes-air-filter.jpg', 40);

-- Insert sample contact messages
INSERT INTO ContactMessages (Name, Email, Subject, Message) VALUES
('Alice Johnson', 'alice@email.com', 'Inquiry about Toyota Camry', 'Hi, I am interested in the 2023 Toyota Camry. Could you provide more details about its features and availability?'),
('Bob Wilson', 'bob@email.com', 'Car Part Compatibility', 'I need to know if the brake pads TOY-FBP-002 are compatible with my 2020 Toyota RAV4.'),
('Carol Brown', 'carol@email.com', 'Service Request', 'I recently purchased a car from your dealership and need information about maintenance services.');

-- =====================================================
-- INDEXES FOR PERFORMANCE
-- =====================================================

-- Additional composite indexes for better query performance
CREATE INDEX idx_cars_brand_category ON Cars(BrandId, CategoryId);
CREATE INDEX idx_cars_price_year ON Cars(Price, Year);
CREATE INDEX idx_cars_fuel_transmission ON Cars(FuelType, Transmission);
CREATE INDEX idx_cars_available_stock ON Cars(IsAvailable, StockQuantity);

CREATE INDEX idx_carparts_brand_category ON CarParts(BrandId, CategoryId);
CREATE INDEX idx_carparts_price_stock ON CarParts(Price, StockQuantity);
CREATE INDEX idx_carparts_available_stock ON CarParts(IsAvailable, StockQuantity);

CREATE INDEX idx_orders_user_status ON Orders(UserId, OrderStatus);
CREATE INDEX idx_orders_date_status ON Orders(OrderDate, OrderStatus);
CREATE INDEX idx_orders_payment_status ON Orders(PaymentStatus, OrderStatus);

CREATE INDEX idx_orderitems_order_type ON OrderItems(OrderId, ItemType);
CREATE INDEX idx_orderitems_type_item ON OrderItems(ItemType, ItemId);

CREATE INDEX idx_cart_user_type ON Cart(UserId, ItemType);

-- =====================================================
-- ANALYZE TABLES FOR OPTIMIZATION
-- =====================================================
ANALYZE TABLE Users, Brands, Categories, Cars, CarParts, Orders, OrderItems, Cart, ContactMessages;

-- =====================================================
-- SCRIPT COMPLETION MESSAGE
-- =====================================================
SELECT 'ABC Car Traders Database Script completed successfully!' as 'Status';
SELECT 'Database: ABCCarTradersDB' as 'Database Name';
SELECT 'Tables Created: 8' as 'Tables Count';
SELECT 'Views Created: 3' as 'Views Count';
SELECT 'Triggers Created: 5' as 'Triggers Count';
SELECT NOW() as 'Creation Time';
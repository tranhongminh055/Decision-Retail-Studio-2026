
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DSS_SalesDB')
BEGIN
    CREATE DATABASE DSS_SalesDB
    COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT '>> Database [DSS_SalesDB] da duoc tao thanh cong.';
END
ELSE
BEGIN
    PRINT '>> Database [DSS_SalesDB] da ton tai san.';
END
GO

USE DSS_SalesDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.DECISION_RESULTS', N'U') IS NOT NULL DROP TABLE dbo.DECISION_RESULTS;
IF OBJECT_ID(N'dbo.ALTERNATIVE_SCORES', N'U') IS NOT NULL DROP TABLE dbo.ALTERNATIVE_SCORES;
IF OBJECT_ID(N'dbo.PAIRWISE_COMPARISONS', N'U') IS NOT NULL DROP TABLE dbo.PAIRWISE_COMPARISONS;
IF OBJECT_ID(N'dbo.ALTERNATIVES', N'U') IS NOT NULL DROP TABLE dbo.ALTERNATIVES;
IF OBJECT_ID(N'dbo.CRITERIA', N'U') IS NOT NULL DROP TABLE dbo.CRITERIA;
IF OBJECT_ID(N'dbo.DECISION_SESSIONS', N'U') IS NOT NULL DROP TABLE dbo.DECISION_SESSIONS;
IF OBJECT_ID(N'dbo.ORDER_DETAILS', N'U') IS NOT NULL DROP TABLE dbo.ORDER_DETAILS;
IF OBJECT_ID(N'dbo.ORDERS', N'U') IS NOT NULL DROP TABLE dbo.ORDERS;
IF OBJECT_ID(N'dbo.CUSTOMERS', N'U') IS NOT NULL DROP TABLE dbo.CUSTOMERS;
IF OBJECT_ID(N'dbo.PRODUCTS', N'U') IS NOT NULL DROP TABLE dbo.PRODUCTS;
IF OBJECT_ID(N'dbo.CATEGORIES', N'U') IS NOT NULL DROP TABLE dbo.CATEGORIES;
IF OBJECT_ID(N'dbo.USERS', N'U') IS NOT NULL DROP TABLE dbo.USERS;
GO

CREATE TABLE dbo.USERS (
    user_id          INT IDENTITY(1,1) NOT NULL,
    username         VARCHAR(50) NOT NULL,
    password_hash    VARCHAR(255) NOT NULL,
    full_name        NVARCHAR(100) NOT NULL,
    email            VARCHAR(100) NOT NULL,
    phone_number     VARCHAR(20) NULL,
    role             VARCHAR(20) NOT NULL,
    status           BIT NOT NULL CONSTRAINT DF_Users_Status DEFAULT 1,
    created_at       DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (user_id),
    CONSTRAINT UQ_Users_Username UNIQUE (username),
    CONSTRAINT UQ_Users_Email UNIQUE (email),
    CONSTRAINT CK_Users_Role CHECK (role IN ('Admin', 'DecisionMaker', 'SalesManager', 'SalesStaff'))
);
GO

CREATE TABLE dbo.CATEGORIES (
    category_id      INT IDENTITY(1,1) NOT NULL,
    category_code    VARCHAR(30) NOT NULL,
    category_name    NVARCHAR(100) NOT NULL,
    description      NVARCHAR(500) NULL,
    status           BIT NOT NULL CONSTRAINT DF_Categories_Status DEFAULT 1,

    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (category_id),
    CONSTRAINT UQ_Categories_Code UNIQUE (category_code)
);
GO

CREATE TABLE dbo.PRODUCTS (
    product_id       INT IDENTITY(1,1) NOT NULL,
    category_id      INT NOT NULL,
    product_code     VARCHAR(50) NOT NULL,
    product_name     NVARCHAR(200) NOT NULL,
    cost_price       DECIMAL(18,2) NOT NULL,
    selling_price    DECIMAL(18,2) NOT NULL,
    stock_quantity   INT NOT NULL CONSTRAINT DF_Products_Stock DEFAULT 0,
    min_stock_level  INT NOT NULL CONSTRAINT DF_Products_MinStock DEFAULT 10,
    lead_time_days   INT NOT NULL CONSTRAINT DF_Products_LeadTime DEFAULT 7,
    is_active        BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT 1,
    created_at       DATETIME2 NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (product_id),
    CONSTRAINT UQ_Products_Code UNIQUE (product_code),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (category_id) REFERENCES dbo.CATEGORIES(category_id),
    CONSTRAINT CK_Products_CostPrice CHECK (cost_price >= 0),
    CONSTRAINT CK_Products_SellingPrice CHECK (selling_price >= cost_price),
    CONSTRAINT CK_Products_StockQuantity CHECK (stock_quantity >= 0)
);
GO

CREATE TABLE dbo.CUSTOMERS (
    customer_id      INT IDENTITY(1,1) NOT NULL,
    customer_code    VARCHAR(30) NOT NULL,
    customer_name    NVARCHAR(150) NOT NULL,
    phone            VARCHAR(20) NULL,
    email            VARCHAR(100) NULL,
    address          NVARCHAR(255) NULL,
    customer_segment VARCHAR(20) NOT NULL,
    total_spent      DECIMAL(18,2) NOT NULL CONSTRAINT DF_Customers_TotalSpent DEFAULT 0,

    CONSTRAINT PK_Customers PRIMARY KEY CLUSTERED (customer_id),
    CONSTRAINT UQ_Customers_Code UNIQUE (customer_code),
    CONSTRAINT UQ_Customers_Phone UNIQUE (phone),
    CONSTRAINT CK_Customers_Segment CHECK (customer_segment IN ('Retail', 'Wholesale', 'VIP'))
);
GO

CREATE TABLE dbo.ORDERS (
    order_id         INT IDENTITY(1,1) NOT NULL,
    order_code       VARCHAR(50) NOT NULL,
    customer_id      INT NOT NULL,
    user_id          INT NOT NULL,
    order_date       DATETIME2 NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT SYSDATETIME(),
    order_status     VARCHAR(20) NOT NULL,
    total_amount     DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_TotalAmount DEFAULT 0,
    discount_amount  DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_Discount DEFAULT 0,
    final_amount     DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_FinalAmount DEFAULT 0,

    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (order_id),
    CONSTRAINT UQ_Orders_Code UNIQUE (order_code),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (customer_id) REFERENCES dbo.CUSTOMERS(customer_id),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (user_id) REFERENCES dbo.USERS(user_id),
    CONSTRAINT CK_Orders_Status CHECK (order_status IN ('Pending', 'Completed', 'Cancelled')),
    CONSTRAINT CK_Orders_Amounts CHECK (total_amount >= 0 AND discount_amount >= 0 AND final_amount >= 0)
);
GO

CREATE TABLE dbo.ORDER_DETAILS (
    order_detail_id  INT IDENTITY(1,1) NOT NULL,
    order_id         INT NOT NULL,
    product_id       INT NOT NULL,
    quantity         INT NOT NULL,
    unit_price       DECIMAL(18,2) NOT NULL,
    unit_cost        DECIMAL(18,2) NOT NULL,
    discount_rate    DECIMAL(5,2) NOT NULL CONSTRAINT DF_OrderDetails_DiscountRate DEFAULT 0,
    line_total       AS CAST(((quantity * unit_price) * (1.0 - (discount_rate / 100.0))) AS DECIMAL(18,2)) PERSISTED,

    CONSTRAINT PK_OrderDetails PRIMARY KEY CLUSTERED (order_detail_id),
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (order_id) REFERENCES dbo.ORDERS(order_id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (product_id) REFERENCES dbo.PRODUCTS(product_id),
    CONSTRAINT CK_OrderDetails_Quantity CHECK (quantity > 0),
    CONSTRAINT CK_OrderDetails_UnitPrice CHECK (unit_price >= 0),
    CONSTRAINT CK_OrderDetails_UnitCost CHECK (unit_cost >= 0),
    CONSTRAINT CK_OrderDetails_DiscountRate CHECK (discount_rate >= 0 AND discount_rate <= 100)
);
GO

CREATE TABLE dbo.DECISION_SESSIONS (
    session_id          INT IDENTITY(1,1) NOT NULL,
    session_code        VARCHAR(50) NOT NULL,
    session_title       NVARCHAR(200) NOT NULL,
    description         NVARCHAR(MAX) NULL,
    created_by          INT NOT NULL,
    start_date          DATETIME2 NOT NULL CONSTRAINT DF_Sessions_StartDate DEFAULT SYSDATETIME(),
    end_date            DATETIME2 NULL,
    consistency_ratio   DECIMAL(8,4) NULL,
    is_consistent       AS (CASE WHEN consistency_ratio IS NOT NULL AND consistency_ratio <= 0.10 THEN 1 ELSE 0 END) PERSISTED,
    status              VARCHAR(20) NOT NULL CONSTRAINT DF_Sessions_Status DEFAULT 'Draft',

    CONSTRAINT PK_DecisionSessions PRIMARY KEY CLUSTERED (session_id),
    CONSTRAINT UQ_DecisionSessions_Code UNIQUE (session_code),
    CONSTRAINT FK_DecisionSessions_Users FOREIGN KEY (created_by) REFERENCES dbo.USERS(user_id),
    CONSTRAINT CK_DecisionSessions_Status CHECK (status IN ('Draft', 'Calculated', 'Approved', 'Rejected'))
);
GO

CREATE TABLE dbo.CRITERIA (
    criterion_id        INT IDENTITY(1,1) NOT NULL,
    parent_id           INT NULL,
    criterion_code      VARCHAR(30) NOT NULL,
    criterion_name      NVARCHAR(150) NOT NULL,
    description         NVARCHAR(500) NULL,
    criterion_type      VARCHAR(10) NOT NULL,
    data_source_type    VARCHAR(30) NOT NULL,
    is_active           BIT NOT NULL CONSTRAINT DF_Criteria_IsActive DEFAULT 1,

    CONSTRAINT PK_Criteria PRIMARY KEY CLUSTERED (criterion_id),
    CONSTRAINT UQ_Criteria_Code UNIQUE (criterion_code),
    CONSTRAINT FK_Criteria_Parent FOREIGN KEY (parent_id) REFERENCES dbo.CRITERIA(criterion_id),
    CONSTRAINT CK_Criteria_Type CHECK (criterion_type IN ('Benefit', 'Cost')),
    CONSTRAINT CK_Criteria_Source CHECK (data_source_type IN ('Calculated_OLTP', 'Expert_Survey', 'Market_Data'))
);
GO

CREATE TABLE dbo.ALTERNATIVES (
    alternative_id      INT IDENTITY(1,1) NOT NULL,
    session_id          INT NOT NULL,
    product_id          INT NULL,
    alternative_code    VARCHAR(50) NOT NULL,
    alternative_name    NVARCHAR(200) NOT NULL,
    description         NVARCHAR(1000) NULL,
    notes               NVARCHAR(500) NULL,

    CONSTRAINT PK_Alternatives PRIMARY KEY CLUSTERED (alternative_id),
    CONSTRAINT FK_Alternatives_Sessions FOREIGN KEY (session_id) REFERENCES dbo.DECISION_SESSIONS(session_id) ON DELETE CASCADE,
    CONSTRAINT FK_Alternatives_Products FOREIGN KEY (product_id) REFERENCES dbo.PRODUCTS(product_id),
    CONSTRAINT UQ_Alternatives_SessionCode UNIQUE (session_id, alternative_code)
);
GO

CREATE TABLE dbo.PAIRWISE_COMPARISONS (
    comparison_id       INT IDENTITY(1,1) NOT NULL,
    session_id          INT NOT NULL,
    criterion_id_1      INT NOT NULL,
    criterion_id_2      INT NOT NULL,
    saaty_value         DECIMAL(10,5) NOT NULL,
    evaluation_notes    NVARCHAR(255) NULL,
    evaluated_by        INT NOT NULL,

    CONSTRAINT PK_PairwiseComparisons PRIMARY KEY CLUSTERED (comparison_id),
    CONSTRAINT FK_Pairwise_Sessions FOREIGN KEY (session_id) REFERENCES dbo.DECISION_SESSIONS(session_id) ON DELETE CASCADE,
    CONSTRAINT FK_Pairwise_Criterion1 FOREIGN KEY (criterion_id_1) REFERENCES dbo.CRITERIA(criterion_id),
    CONSTRAINT FK_Pairwise_Criterion2 FOREIGN KEY (criterion_id_2) REFERENCES dbo.CRITERIA(criterion_id),
    CONSTRAINT FK_Pairwise_Users FOREIGN KEY (evaluated_by) REFERENCES dbo.USERS(user_id),
    CONSTRAINT CK_Pairwise_SaatyValue CHECK (saaty_value > 0),
    CONSTRAINT UQ_Pairwise_Session_C1_C2 UNIQUE (session_id, criterion_id_1, criterion_id_2)
);
GO

CREATE TABLE dbo.ALTERNATIVE_SCORES (
    score_id            INT IDENTITY(1,1) NOT NULL,
    session_id          INT NOT NULL,
    alternative_id      INT NOT NULL,
    criterion_id        INT NOT NULL,
    raw_value           DECIMAL(18,4) NOT NULL,
    normalized_score    DECIMAL(10,6) NOT NULL,

    CONSTRAINT PK_AlternativeScores PRIMARY KEY CLUSTERED (score_id),
    CONSTRAINT FK_AltScores_Sessions FOREIGN KEY (session_id) REFERENCES dbo.DECISION_SESSIONS(session_id) ON DELETE CASCADE,
    CONSTRAINT FK_AltScores_Alternatives FOREIGN KEY (alternative_id) REFERENCES dbo.ALTERNATIVES(alternative_id),
    CONSTRAINT FK_AltScores_Criteria FOREIGN KEY (criterion_id) REFERENCES dbo.CRITERIA(criterion_id),
    CONSTRAINT CK_AltScores_Normalized CHECK (normalized_score >= 0.0 AND normalized_score <= 1.0),
    CONSTRAINT UQ_AltScores_Session_Alt_Crit UNIQUE (session_id, alternative_id, criterion_id)
);
GO

CREATE TABLE dbo.DECISION_RESULTS (
    result_id           INT IDENTITY(1,1) NOT NULL,
    session_id          INT NOT NULL,
    alternative_id      INT NOT NULL,
    final_score         DECIMAL(10,6) NOT NULL,
    ranking             INT NOT NULL,
    decision_outcome    VARCHAR(30) NOT NULL,
    decision_date       DATETIME2 NOT NULL CONSTRAINT DF_Results_Date DEFAULT SYSDATETIME(),
    comments            NVARCHAR(500) NULL,

    CONSTRAINT PK_DecisionResults PRIMARY KEY CLUSTERED (result_id),
    CONSTRAINT FK_Results_Sessions FOREIGN KEY (session_id) REFERENCES dbo.DECISION_SESSIONS(session_id) ON DELETE CASCADE,
    CONSTRAINT FK_Results_Alternatives FOREIGN KEY (alternative_id) REFERENCES dbo.ALTERNATIVES(alternative_id),
    CONSTRAINT CK_Results_Score CHECK (final_score >= 0.0 AND final_score <= 1.0),
    CONSTRAINT CK_Results_Ranking CHECK (ranking > 0),
    CONSTRAINT CK_Results_Outcome CHECK (decision_outcome IN ('Selected', 'UnderReview', 'Rejected')),
    CONSTRAINT UQ_Results_Session_Alt UNIQUE (session_id, alternative_id)
);
GO

CREATE NONCLUSTERED INDEX IX_Products_CategoryId ON dbo.PRODUCTS(category_id);
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId ON dbo.ORDERS(customer_id);
CREATE NONCLUSTERED INDEX IX_Orders_OrderDate ON dbo.ORDERS(order_date);
CREATE NONCLUSTERED INDEX IX_OrderDetails_ProductId ON dbo.ORDER_DETAILS(product_id);
CREATE NONCLUSTERED INDEX IX_OrderDetails_OrderId ON dbo.ORDER_DETAILS(order_id);
CREATE NONCLUSTERED INDEX IX_Alternatives_SessionId ON dbo.ALTERNATIVES(session_id);
CREATE NONCLUSTERED INDEX IX_Pairwise_SessionId ON dbo.PAIRWISE_COMPARISONS(session_id);
CREATE NONCLUSTERED INDEX IX_AltScores_SessionId ON dbo.ALTERNATIVE_SCORES(session_id);
CREATE NONCLUSTERED INDEX IX_Results_SessionRanking ON dbo.DECISION_RESULTS(session_id, ranking);
GO

PRINT '>> Kich ban DDL khoi tao CSDL [DSS_SalesDB] da thuc thi hoan tat.';
GO

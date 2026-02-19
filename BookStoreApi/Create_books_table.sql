CREATE TABLE IF NOT EXISTS books (
                       Id INT AUTO_INCREMENT PRIMARY KEY,
                       Title VARCHAR(255) NOT NULL,
                       Author VARCHAR(255) NOT NULL,
                       PublicationYear INT,
                       ISBN VARCHAR(20),
                       InStock INT DEFAULT 0
);
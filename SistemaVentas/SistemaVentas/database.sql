CREATE DATABASE IF NOT EXISTS ventasdb;
USE ventasdb;
CREATE TABLE usuarios (id INT AUTO_INCREMENT PRIMARY KEY, usuario VARCHAR(50) UNIQUE, clave VARCHAR(100));
CREATE TABLE productos (codigo VARCHAR(10) PRIMARY KEY, nombre VARCHAR(100) NOT NULL, descripcion VARCHAR(200), precio DECIMAL(10,2) NOT NULL, stock INT NOT NULL);
CREATE TABLE clientes (id INT AUTO_INCREMENT PRIMARY KEY, nombre VARCHAR(100) NOT NULL, telefono VARCHAR(20), correo VARCHAR(100));
CREATE TABLE ventas (id INT AUTO_INCREMENT PRIMARY KEY, fecha DATETIME NOT NULL, cliente_id INT NOT NULL, total DECIMAL(10,2) NOT NULL, FOREIGN KEY (cliente_id) REFERENCES clientes(id));
CREATE TABLE detalle_venta (id INT AUTO_INCREMENT PRIMARY KEY, venta_id INT NOT NULL, producto_codigo VARCHAR(10) NOT NULL, cantidad INT NOT NULL, precio DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (venta_id) REFERENCES ventas(id), FOREIGN KEY (producto_codigo) REFERENCES productos(codigo));
INSERT INTO usuarios (usuario, clave) VALUES ('admin','1234');
INSERT INTO productos VALUES ('P001','Teclado mecánico','Switch rojo, USB',32500,15),('P002','Mouse inalámbrico','2.4 GHz',12900,30),('P003','Monitor 24"','IPS Full HD',89000,6);
INSERT INTO clientes (nombre,telefono,correo) VALUES ('Ana Rodríguez','8888-1234','ana@correo.com'),('Carlos Mora','8777-5678','carlos@correo.com');

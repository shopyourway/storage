CREATE DATABASE IF NOT EXISTS `storage`
    CHARACTER SET utf8
    COLLATE utf8_unicode_ci;

USE `storage`;

/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` bigint(10) NOT NULL,
  `name` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
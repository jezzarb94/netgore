-- MySQL dump 10.13  Distrib 5.1.38, for Win64 (unknown)
--
-- Host: localhost    Database: demogame
-- ------------------------------------------------------
-- Server version	5.1.38-community
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO,MYSQL40' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `id` int(11) NOT NULL COMMENT 'The account ID.',
  `name` varchar(30) NOT NULL COMMENT 'The account name.',
  `password` varchar(40) NOT NULL COMMENT 'The account password.',
  `email` varchar(60) NOT NULL COMMENT 'The email address.',
  `time_created` datetime NOT NULL COMMENT 'The DateTime of when the account was created.',
  `time_last_login` datetime NOT NULL COMMENT 'The DateTime that the account was last logged in to.',
  `creator_ip` int(10) unsigned NOT NULL COMMENT 'The IP address that created the account.',
  `current_ip` int(10) unsigned DEFAULT NULL COMMENT 'IP address currently logged in to the account, or null if nobody is logged in.',
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) TYPE=InnoDB;

--
-- Dumping data for table `account`
--

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES (0,'Test','3fc0a7acf087f549ac2b266baf94b8b1','test@test.com','2010-02-11 17:52:28','2010-02-11 18:03:56',16777343,NULL),(1,'Spodi','3fc0a7acf087f549ac2b266baf94b8b1','spodi@netgore.com','2009-09-07 15:43:16','2010-06-21 12:43:22',16777343,NULL);
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `account_ips`
--

DROP TABLE IF EXISTS `account_ips`;
CREATE TABLE `account_ips` (
  `account_id` int(11) NOT NULL COMMENT 'The ID of the account.',
  `ip` int(10) unsigned NOT NULL COMMENT 'The IP that logged into the account.',
  `time` datetime NOT NULL COMMENT 'When this IP last logged into this account.',
  PRIMARY KEY (`account_id`,`time`),
  KEY `account_id` (`account_id`,`ip`),
  CONSTRAINT `account_ips_ibfk_1` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `account_ips`
--

LOCK TABLES `account_ips` WRITE;
/*!40000 ALTER TABLE `account_ips` DISABLE KEYS */;
INSERT INTO `account_ips` VALUES (1,16777343,'2010-06-03 22:51:10'),(1,16777343,'2010-06-07 17:58:12'),(1,16777343,'2010-06-07 18:30:03'),(1,16777343,'2010-06-07 18:32:47'),(1,16777343,'2010-06-07 18:37:03'),(1,16777343,'2010-06-07 18:38:14'),(1,16777343,'2010-06-07 18:39:30'),(1,16777343,'2010-06-07 18:41:12'),(1,16777343,'2010-06-07 18:44:11'),(1,16777343,'2010-06-11 02:14:30'),(1,16777343,'2010-06-15 19:02:11'),(1,16777343,'2010-06-15 19:03:40'),(1,16777343,'2010-06-15 19:05:29'),(1,16777343,'2010-06-17 11:45:48'),(1,16777343,'2010-06-17 11:58:09'),(1,16777343,'2010-06-17 12:11:10'),(1,16777343,'2010-06-17 12:13:11'),(1,16777343,'2010-06-17 12:20:52'),(1,16777343,'2010-06-17 12:28:01'),(1,16777343,'2010-06-17 12:32:53'),(1,16777343,'2010-06-17 13:35:27'),(1,16777343,'2010-06-17 15:39:12'),(1,16777343,'2010-06-17 15:48:20'),(1,16777343,'2010-06-17 20:19:56'),(1,16777343,'2010-06-17 20:27:06'),(1,16777343,'2010-06-17 20:33:18'),(1,16777343,'2010-06-17 20:34:19'),(1,16777343,'2010-06-17 20:50:20'),(1,16777343,'2010-06-17 20:54:21'),(1,16777343,'2010-06-17 20:56:32'),(1,16777343,'2010-06-17 20:56:43'),(1,16777343,'2010-06-17 21:01:03'),(1,16777343,'2010-06-21 11:47:41'),(1,16777343,'2010-06-21 12:34:41'),(1,16777343,'2010-06-21 12:37:15'),(1,16777343,'2010-06-21 12:43:22');
/*!40000 ALTER TABLE `account_ips` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alliance`
--

DROP TABLE IF EXISTS `alliance`;
CREATE TABLE `alliance` (
  `id` tinyint(3) unsigned NOT NULL,
  `name` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `alliance`
--

LOCK TABLES `alliance` WRITE;
/*!40000 ALTER TABLE `alliance` DISABLE KEYS */;
INSERT INTO `alliance` VALUES (0,'user'),(1,'monster'),(2,'townsperson'),(3,'aggressive monster');
/*!40000 ALTER TABLE `alliance` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alliance_attackable`
--

DROP TABLE IF EXISTS `alliance_attackable`;
CREATE TABLE `alliance_attackable` (
  `alliance_id` tinyint(3) unsigned NOT NULL,
  `attackable_id` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`alliance_id`,`attackable_id`),
  KEY `attackable_id` (`attackable_id`),
  KEY `alliance_id` (`alliance_id`),
  CONSTRAINT `alliance_attackable_ibfk_3` FOREIGN KEY (`attackable_id`) REFERENCES `alliance` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `alliance_attackable_ibfk_4` FOREIGN KEY (`alliance_id`) REFERENCES `alliance` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `alliance_attackable`
--

LOCK TABLES `alliance_attackable` WRITE;
/*!40000 ALTER TABLE `alliance_attackable` DISABLE KEYS */;
INSERT INTO `alliance_attackable` VALUES (1,0),(0,1),(3,1),(3,3);
/*!40000 ALTER TABLE `alliance_attackable` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alliance_hostile`
--

DROP TABLE IF EXISTS `alliance_hostile`;
CREATE TABLE `alliance_hostile` (
  `alliance_id` tinyint(3) unsigned NOT NULL,
  `hostile_id` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`alliance_id`,`hostile_id`),
  KEY `hostile_id` (`hostile_id`),
  KEY `alliance_id` (`alliance_id`),
  CONSTRAINT `alliance_hostile_ibfk_3` FOREIGN KEY (`hostile_id`) REFERENCES `alliance` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `alliance_hostile_ibfk_4` FOREIGN KEY (`alliance_id`) REFERENCES `alliance` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `alliance_hostile`
--

LOCK TABLES `alliance_hostile` WRITE;
/*!40000 ALTER TABLE `alliance_hostile` DISABLE KEYS */;
INSERT INTO `alliance_hostile` VALUES (1,0),(0,1),(3,1),(3,3);
/*!40000 ALTER TABLE `alliance_hostile` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character`
--

DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` (
  `id` int(11) NOT NULL,
  `account_id` int(11) DEFAULT NULL,
  `character_template_id` smallint(5) unsigned DEFAULT NULL,
  `name` varchar(30) NOT NULL,
  `permissions` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `shop_id` smallint(5) unsigned DEFAULT NULL,
  `chat_dialog` smallint(5) unsigned DEFAULT NULL,
  `ai_id` smallint(5) unsigned DEFAULT NULL,
  `load_map_id` smallint(5) unsigned NOT NULL DEFAULT '1',
  `load_x` smallint(5) unsigned NOT NULL DEFAULT '50',
  `load_y` smallint(5) unsigned NOT NULL DEFAULT '50',
  `respawn_map_id` smallint(5) unsigned DEFAULT '1',
  `respawn_x` float NOT NULL DEFAULT '50',
  `respawn_y` float NOT NULL DEFAULT '50',
  `body_id` smallint(5) unsigned NOT NULL DEFAULT '1',
  `move_speed` smallint(5) unsigned NOT NULL DEFAULT '1800',
  `cash` int(11) NOT NULL DEFAULT '0',
  `level` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `exp` int(11) NOT NULL DEFAULT '0',
  `statpoints` int(11) NOT NULL DEFAULT '0',
  `hp` smallint(6) NOT NULL DEFAULT '50',
  `mp` smallint(6) NOT NULL DEFAULT '50',
  `stat_maxhp` smallint(6) NOT NULL DEFAULT '50',
  `stat_maxmp` smallint(6) NOT NULL DEFAULT '50',
  `stat_minhit` smallint(6) NOT NULL DEFAULT '1',
  `stat_maxhit` smallint(6) NOT NULL DEFAULT '1',
  `stat_defence` smallint(6) NOT NULL DEFAULT '1',
  `stat_agi` smallint(6) NOT NULL DEFAULT '1',
  `stat_int` smallint(6) NOT NULL DEFAULT '1',
  `stat_str` smallint(6) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `template_id` (`character_template_id`),
  KEY `character_ibfk_2` (`load_map_id`),
  KEY `idx_name` (`name`),
  KEY `account_id` (`account_id`),
  KEY `shop_id` (`shop_id`),
  KEY `character_ibfk_5` (`respawn_map_id`),
  CONSTRAINT `character_ibfk_1` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_ibfk_2` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_ibfk_3` FOREIGN KEY (`shop_id`) REFERENCES `shop` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_ibfk_4` FOREIGN KEY (`load_map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_ibfk_5` FOREIGN KEY (`respawn_map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character`
--

LOCK TABLES `character` WRITE;
/*!40000 ALTER TABLE `character` DISABLE KEYS */;
INSERT INTO `character` VALUES (0,0,NULL,'Test',0,NULL,NULL,NULL,1,765,45,1,765,45,1,1800,0,1,0,0,50,50,50,50,1,1,1,1,1,1),(1,1,NULL,'Spodi',255,NULL,NULL,NULL,1,765,45,1,765,45,1,1800,201385,30,880,143,100,100,100,100,1,1,1,1,3,1),(2,NULL,1,'Test A',0,NULL,NULL,1,2,535,1201,2,800,250,1,1800,3012,12,810,527,3,5,5,5,5,5,0,5,5,5),(3,NULL,1,'Test B',0,NULL,NULL,1,2,3,1330,2,500,250,1,1800,3012,12,810,527,5,5,5,5,5,5,0,5,5,5);
/*!40000 ALTER TABLE `character` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_equipped`
--

DROP TABLE IF EXISTS `character_equipped`;
CREATE TABLE `character_equipped` (
  `character_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`character_id`,`slot`),
  KEY `item_id` (`item_id`),
  CONSTRAINT `character_equipped_ibfk_3` FOREIGN KEY (`item_id`) REFERENCES `item` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_equipped_ibfk_4` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_equipped`
--

LOCK TABLES `character_equipped` WRITE;
/*!40000 ALTER TABLE `character_equipped` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_equipped` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_inventory`
--

DROP TABLE IF EXISTS `character_inventory`;
CREATE TABLE `character_inventory` (
  `character_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`character_id`,`slot`),
  KEY `item_id` (`item_id`),
  KEY `character_id` (`character_id`),
  CONSTRAINT `character_inventory_ibfk_3` FOREIGN KEY (`item_id`) REFERENCES `item` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_inventory_ibfk_4` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_inventory`
--

LOCK TABLES `character_inventory` WRITE;
/*!40000 ALTER TABLE `character_inventory` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_quest_status`
--

DROP TABLE IF EXISTS `character_quest_status`;
CREATE TABLE `character_quest_status` (
  `character_id` int(11) NOT NULL,
  `quest_id` smallint(5) unsigned NOT NULL,
  `started_on` datetime NOT NULL,
  `completed_on` datetime DEFAULT NULL,
  PRIMARY KEY (`character_id`,`quest_id`)
) TYPE=InnoDB;

--
-- Dumping data for table `character_quest_status`
--

LOCK TABLES `character_quest_status` WRITE;
/*!40000 ALTER TABLE `character_quest_status` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_quest_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_quest_status_kills`
--

DROP TABLE IF EXISTS `character_quest_status_kills`;
CREATE TABLE `character_quest_status_kills` (
  `character_id` int(11) NOT NULL,
  `quest_id` smallint(5) unsigned NOT NULL,
  `character_template_id` smallint(5) unsigned NOT NULL,
  `count` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`character_id`,`quest_id`,`character_template_id`),
  KEY `quest_id` (`quest_id`),
  KEY `character_template_id` (`character_template_id`),
  CONSTRAINT `character_quest_status_kills_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_quest_status_kills_ibfk_2` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_quest_status_kills_ibfk_3` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_quest_status_kills`
--

LOCK TABLES `character_quest_status_kills` WRITE;
/*!40000 ALTER TABLE `character_quest_status_kills` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_quest_status_kills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_status_effect`
--

DROP TABLE IF EXISTS `character_status_effect`;
CREATE TABLE `character_status_effect` (
  `id` int(11) NOT NULL COMMENT 'Unique ID of the status effect instance.',
  `character_id` int(11) NOT NULL COMMENT 'ID of the Character that the status effect is on.',
  `status_effect_id` tinyint(3) unsigned NOT NULL COMMENT 'ID of the status effect that this effect is for. This corresponds to the StatusEffectType enum''s value.',
  `power` smallint(5) unsigned NOT NULL COMMENT 'The power of this status effect instance.',
  `time_left_secs` smallint(5) unsigned NOT NULL COMMENT 'The amount of time remaining for this status effect in seconds.',
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  CONSTRAINT `character_status_effect_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_status_effect`
--

LOCK TABLES `character_status_effect` WRITE;
/*!40000 ALTER TABLE `character_status_effect` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_status_effect` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_template`
--

DROP TABLE IF EXISTS `character_template`;
CREATE TABLE `character_template` (
  `id` smallint(5) unsigned NOT NULL,
  `alliance_id` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `name` varchar(50) NOT NULL DEFAULT 'New NPC',
  `ai_id` smallint(5) unsigned DEFAULT NULL,
  `shop_id` smallint(5) unsigned DEFAULT NULL,
  `chat_dialog` smallint(5) unsigned DEFAULT NULL,
  `body_id` smallint(5) unsigned NOT NULL DEFAULT '1',
  `move_speed` smallint(5) unsigned NOT NULL DEFAULT '1800',
  `respawn` smallint(5) unsigned NOT NULL DEFAULT '5',
  `level` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `exp` int(11) NOT NULL DEFAULT '0',
  `statpoints` int(11) NOT NULL DEFAULT '0',
  `give_exp` smallint(5) unsigned NOT NULL DEFAULT '0',
  `give_cash` smallint(5) unsigned NOT NULL DEFAULT '0',
  `stat_maxhp` smallint(6) NOT NULL DEFAULT '50',
  `stat_maxmp` smallint(6) NOT NULL DEFAULT '50',
  `stat_minhit` smallint(6) NOT NULL DEFAULT '1',
  `stat_maxhit` smallint(6) NOT NULL DEFAULT '1',
  `stat_defence` smallint(6) NOT NULL DEFAULT '1',
  `stat_agi` smallint(6) NOT NULL DEFAULT '1',
  `stat_int` smallint(6) NOT NULL DEFAULT '1',
  `stat_str` smallint(6) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `alliance_id` (`alliance_id`),
  KEY `shop_id` (`shop_id`),
  CONSTRAINT `character_template_ibfk_2` FOREIGN KEY (`alliance_id`) REFERENCES `alliance` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_template_ibfk_3` FOREIGN KEY (`shop_id`) REFERENCES `shop` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_template`
--

LOCK TABLES `character_template` WRITE;
/*!40000 ALTER TABLE `character_template` DISABLE KEYS */;
INSERT INTO `character_template` VALUES (0,0,'User Template',NULL,NULL,NULL,1,1800,5,1,0,0,0,0,50,50,1,2,1,1,1,1),(1,1,'A Test NPC',1,NULL,NULL,2,1800,10,1,0,0,5,5,5,5,1,2,1,1,1,1),(2,2,'Quest Giver',NULL,NULL,NULL,2,1800,5,1,0,0,0,0,50,50,1,1,1,1,1,1),(4,2,'Potion Vendor',NULL,1,NULL,2,1800,5,1,0,0,0,0,50,50,1,1,1,1,1,1),(5,2,'Talking Guy',NULL,NULL,0,1,1800,5,1,0,0,0,0,50,50,1,1,1,1,1,1),(6,3,'Brawler',1,NULL,NULL,2,1800,10,1,0,0,8,8,5,5,1,3,1,1,1,1);
/*!40000 ALTER TABLE `character_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_template_equipped`
--

DROP TABLE IF EXISTS `character_template_equipped`;
CREATE TABLE `character_template_equipped` (
  `id` int(11) NOT NULL,
  `character_template_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  `chance` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `item_id` (`item_template_id`),
  KEY `character_id` (`character_template_id`),
  CONSTRAINT `character_template_equipped_ibfk_1` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_template_equipped_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_template_equipped`
--

LOCK TABLES `character_template_equipped` WRITE;
/*!40000 ALTER TABLE `character_template_equipped` DISABLE KEYS */;
INSERT INTO `character_template_equipped` VALUES (0,1,5,3000),(2,1,3,60000);
/*!40000 ALTER TABLE `character_template_equipped` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_template_inventory`
--

DROP TABLE IF EXISTS `character_template_inventory`;
CREATE TABLE `character_template_inventory` (
  `id` int(11) NOT NULL,
  `character_template_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  `min` tinyint(3) unsigned NOT NULL,
  `max` tinyint(3) unsigned NOT NULL,
  `chance` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `item_id` (`item_template_id`),
  KEY `character_id` (`character_template_id`),
  CONSTRAINT `character_template_inventory_ibfk_1` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_template_inventory_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_template_inventory`
--

LOCK TABLES `character_template_inventory` WRITE;
/*!40000 ALTER TABLE `character_template_inventory` DISABLE KEYS */;
INSERT INTO `character_template_inventory` VALUES (0,1,5,0,2,10000),(1,1,7,1,10,65535),(2,1,3,1,1,5000);
/*!40000 ALTER TABLE `character_template_inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_template_quest_provider`
--

DROP TABLE IF EXISTS `character_template_quest_provider`;
CREATE TABLE `character_template_quest_provider` (
  `character_template_id` smallint(5) unsigned NOT NULL,
  `quest_id` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`character_template_id`,`quest_id`),
  KEY `quest_id` (`quest_id`),
  CONSTRAINT `character_template_quest_provider_ibfk_1` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `character_template_quest_provider_ibfk_2` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `character_template_quest_provider`
--

LOCK TABLES `character_template_quest_provider` WRITE;
/*!40000 ALTER TABLE `character_template_quest_provider` DISABLE KEYS */;
INSERT INTO `character_template_quest_provider` VALUES (2,1);
/*!40000 ALTER TABLE `character_template_quest_provider` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `game_constant`
--

DROP TABLE IF EXISTS `game_constant`;
CREATE TABLE `game_constant` (
  `max_characters_per_account` tinyint(3) unsigned NOT NULL,
  `min_account_name_length` tinyint(3) unsigned NOT NULL,
  `max_account_name_length` tinyint(3) unsigned NOT NULL,
  `min_account_password_length` tinyint(3) unsigned NOT NULL,
  `max_account_password_length` tinyint(3) unsigned NOT NULL,
  `min_character_name_length` tinyint(3) unsigned NOT NULL,
  `max_character_name_length` tinyint(3) unsigned NOT NULL,
  `max_shop_items` tinyint(3) unsigned NOT NULL,
  `max_inventory_size` tinyint(3) unsigned NOT NULL,
  `max_status_effect_power` smallint(5) unsigned NOT NULL,
  `screen_width` smallint(5) unsigned NOT NULL,
  `screen_height` smallint(5) unsigned NOT NULL,
  `server_ping_port` smallint(5) unsigned NOT NULL,
  `server_tcp_port` smallint(5) unsigned NOT NULL,
  `server_ip` varchar(150) NOT NULL,
  `world_physics_update_rate` smallint(5) unsigned NOT NULL
) TYPE=InnoDB ROW_FORMAT=COMPACT;

--
-- Dumping data for table `game_constant`
--

LOCK TABLES `game_constant` WRITE;
/*!40000 ALTER TABLE `game_constant` DISABLE KEYS */;
INSERT INTO `game_constant` VALUES (10,3,30,3,30,3,15,36,36,500,1024,768,44446,44445,'127.0.0.1',20);
/*!40000 ALTER TABLE `game_constant` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `guild`
--

DROP TABLE IF EXISTS `guild`;
CREATE TABLE `guild` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(50) NOT NULL,
  `tag` varchar(5) NOT NULL,
  `created` timestamp NOT NULL,
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `guild`
--

LOCK TABLES `guild` WRITE;
/*!40000 ALTER TABLE `guild` DISABLE KEYS */;
INSERT INTO `guild` VALUES (0,'asdfasdf','tg','2010-05-16 19:58:00');
/*!40000 ALTER TABLE `guild` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `guild_event`
--

DROP TABLE IF EXISTS `guild_event`;
CREATE TABLE `guild_event` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'The ID of the event.',
  `guild_id` smallint(5) unsigned NOT NULL COMMENT 'The guild the event took place on.',
  `character_id` int(11) NOT NULL COMMENT 'The character that invoked the event.',
  `target_character_id` int(11) DEFAULT NULL COMMENT 'The optional character that the event involves.',
  `event_id` tinyint(3) unsigned NOT NULL COMMENT 'The ID of the event that took place.',
  `created` datetime NOT NULL COMMENT 'When the event was created.',
  `arg0` varchar(0) DEFAULT NULL COMMENT 'The first optional event argument.',
  `arg1` varchar(0) DEFAULT NULL COMMENT 'The second optional event argument.',
  `arg2` varchar(0) DEFAULT NULL COMMENT 'The third optional event argument.',
  PRIMARY KEY (`id`),
  KEY `guild_id` (`guild_id`),
  KEY `character_id` (`character_id`),
  KEY `target_character_id` (`target_character_id`),
  CONSTRAINT `guild_event_ibfk_1` FOREIGN KEY (`guild_id`) REFERENCES `guild` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `guild_event_ibfk_2` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `guild_event_ibfk_3` FOREIGN KEY (`target_character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `guild_event`
--

LOCK TABLES `guild_event` WRITE;
/*!40000 ALTER TABLE `guild_event` DISABLE KEYS */;
/*!40000 ALTER TABLE `guild_event` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `guild_member`
--

DROP TABLE IF EXISTS `guild_member`;
CREATE TABLE `guild_member` (
  `character_id` int(11) NOT NULL COMMENT 'The character that is a member of the guild.',
  `guild_id` smallint(5) unsigned NOT NULL COMMENT 'The guild the member is a part of.',
  `rank` tinyint(3) unsigned NOT NULL COMMENT 'The member''s ranking in the guild.',
  `joined` datetime NOT NULL COMMENT 'When the member joined the guild.',
  PRIMARY KEY (`character_id`),
  KEY `guild_id` (`guild_id`),
  CONSTRAINT `guild_member_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `guild_member_ibfk_2` FOREIGN KEY (`guild_id`) REFERENCES `guild` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `guild_member`
--

LOCK TABLES `guild_member` WRITE;
/*!40000 ALTER TABLE `guild_member` DISABLE KEYS */;
INSERT INTO `guild_member` VALUES (1,0,3,'2010-01-26 21:34:14');
/*!40000 ALTER TABLE `guild_member` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
CREATE TABLE `item` (
  `id` int(11) NOT NULL,
  `item_template_id` smallint(5) unsigned DEFAULT NULL,
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `weapon_type` tinyint(3) unsigned NOT NULL,
  `range` smallint(5) unsigned NOT NULL,
  `width` tinyint(3) unsigned NOT NULL DEFAULT '16',
  `height` tinyint(3) unsigned NOT NULL DEFAULT '16',
  `name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `amount` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `graphic` smallint(5) unsigned NOT NULL DEFAULT '0',
  `value` int(11) NOT NULL DEFAULT '0',
  `hp` smallint(6) NOT NULL DEFAULT '0',
  `mp` smallint(6) NOT NULL DEFAULT '0',
  `stat_agi` smallint(6) NOT NULL DEFAULT '0',
  `stat_int` smallint(6) NOT NULL DEFAULT '0',
  `stat_str` smallint(6) NOT NULL DEFAULT '0',
  `stat_minhit` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxhit` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxhp` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxmp` smallint(6) NOT NULL DEFAULT '0',
  `stat_defence` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_agi` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_int` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_str` smallint(6) NOT NULL DEFAULT '0',
  `equipped_body` varchar(255) DEFAULT NULL,
  `action_display_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ActionDisplayID to use when using this item.',
  PRIMARY KEY (`id`),
  KEY `item_template_id` (`item_template_id`),
  CONSTRAINT `item_ibfk_1` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `item`
--

LOCK TABLES `item` WRITE;
/*!40000 ALTER TABLE `item` DISABLE KEYS */;
/*!40000 ALTER TABLE `item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item_template`
--

DROP TABLE IF EXISTS `item_template`;
CREATE TABLE `item_template` (
  `id` smallint(5) unsigned NOT NULL,
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `weapon_type` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `range` smallint(5) unsigned NOT NULL DEFAULT '10',
  `width` tinyint(3) unsigned NOT NULL DEFAULT '16',
  `height` tinyint(3) unsigned NOT NULL DEFAULT '16',
  `name` varchar(255) NOT NULL DEFAULT 'New item template',
  `description` varchar(255) NOT NULL DEFAULT ' ',
  `graphic` smallint(5) unsigned NOT NULL DEFAULT '0',
  `value` int(11) NOT NULL DEFAULT '0',
  `hp` smallint(6) NOT NULL DEFAULT '0',
  `mp` smallint(6) NOT NULL DEFAULT '0',
  `stat_agi` smallint(6) NOT NULL DEFAULT '0',
  `stat_int` smallint(6) NOT NULL DEFAULT '0',
  `stat_str` smallint(6) NOT NULL DEFAULT '0',
  `stat_minhit` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxhit` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxhp` smallint(6) NOT NULL DEFAULT '0',
  `stat_maxmp` smallint(6) NOT NULL DEFAULT '0',
  `stat_defence` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_agi` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_int` smallint(6) NOT NULL DEFAULT '0',
  `stat_req_str` smallint(6) NOT NULL DEFAULT '0',
  `equipped_body` varchar(255) DEFAULT NULL,
  `action_display_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ActionDisplayID to use when using this item.',
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `item_template`
--

LOCK TABLES `item_template` WRITE;
/*!40000 ALTER TABLE `item_template` DISABLE KEYS */;
INSERT INTO `item_template` VALUES (0,2,1,10,16,16,'Unarmed','Unarmed',1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,NULL,NULL),(1,1,0,0,9,16,'Healing Potion','A healing potion',94,15,25,0,0,0,0,0,0,0,0,0,0,0,0,NULL,NULL),(2,1,0,0,9,16,'Mana Potion','A mana potion',95,10,0,25,0,0,0,0,0,0,0,0,0,0,0,NULL,NULL),(3,2,1,20,24,24,'Titanium Sword','A sword made out of titanium',96,100,0,0,0,0,0,5,10,0,0,0,0,0,0,NULL,NULL),(4,4,0,0,22,22,'Crystal Armor','Body armor made out of crystal',99,50,0,0,0,0,0,0,0,0,0,5,0,0,0,'crystal body',NULL),(5,3,0,0,11,16,'Crystal Helmet','A helmet made out of crystal',97,50,0,0,0,0,0,0,0,0,0,2,0,0,0,'crystal helmet',NULL),(6,2,2,500,16,16,'Pistol','A pistol that goes BANG BANG SUCKA!',177,500,0,0,0,0,0,25,50,0,0,0,3,3,1,NULL,NULL),(7,2,3,200,11,9,'Rock','Nothing says \"I fight dirty\" quite like a large rock',182,1,0,0,0,0,0,2,6,0,0,0,3,0,8,NULL,1);
/*!40000 ALTER TABLE `item_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `map`
--

DROP TABLE IF EXISTS `map`;
CREATE TABLE `map` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `map`
--

LOCK TABLES `map` WRITE;
/*!40000 ALTER TABLE `map` DISABLE KEYS */;
INSERT INTO `map` VALUES (1,'Desert 1'),(2,'Desert 2');
/*!40000 ALTER TABLE `map` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `map_spawn`
--

DROP TABLE IF EXISTS `map_spawn`;
CREATE TABLE `map_spawn` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `map_id` smallint(5) unsigned NOT NULL,
  `character_template_id` smallint(5) unsigned NOT NULL,
  `amount` tinyint(3) unsigned NOT NULL,
  `x` smallint(5) unsigned DEFAULT NULL,
  `y` smallint(5) unsigned DEFAULT NULL,
  `width` smallint(5) unsigned DEFAULT NULL,
  `height` smallint(5) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_template_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `map_spawn_ibfk_1` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `map_spawn_ibfk_2` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB AUTO_INCREMENT=6;

--
-- Dumping data for table `map_spawn`
--

LOCK TABLES `map_spawn` WRITE;
/*!40000 ALTER TABLE `map_spawn` DISABLE KEYS */;
INSERT INTO `map_spawn` VALUES (0,1,1,3,NULL,NULL,NULL,NULL),(1,1,2,1,190,278,64,64),(3,1,4,1,545,151,64,64),(4,1,5,1,736,58,64,64),(5,2,6,25,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `map_spawn` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `npc_character`
--

DROP TABLE IF EXISTS `npc_character`;
/*!50001 DROP VIEW IF EXISTS `npc_character`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `npc_character` (
  `id` int(11),
  `account_id` int(11),
  `character_template_id` smallint(5) unsigned,
  `name` varchar(30),
  `permissions` tinyint(3) unsigned,
  `shop_id` smallint(5) unsigned,
  `chat_dialog` smallint(5) unsigned,
  `ai_id` smallint(5) unsigned,
  `load_map_id` smallint(5) unsigned,
  `load_x` smallint(5) unsigned,
  `load_y` smallint(5) unsigned,
  `respawn_map_id` smallint(5) unsigned,
  `respawn_x` float,
  `respawn_y` float,
  `body_id` smallint(5) unsigned,
  `move_speed` smallint(5) unsigned,
  `cash` int(11),
  `level` tinyint(3) unsigned,
  `exp` int(11),
  `statpoints` int(11),
  `hp` smallint(6),
  `mp` smallint(6),
  `stat_maxhp` smallint(6),
  `stat_maxmp` smallint(6),
  `stat_minhit` smallint(6),
  `stat_maxhit` smallint(6),
  `stat_defence` smallint(6),
  `stat_agi` smallint(6),
  `stat_int` smallint(6),
  `stat_str` smallint(6)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `quest`
--

DROP TABLE IF EXISTS `quest`;
CREATE TABLE `quest` (
  `id` smallint(5) unsigned NOT NULL,
  `repeatable` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `reward_cash` int(11) NOT NULL DEFAULT '0',
  `reward_exp` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `quest`
--

LOCK TABLES `quest` WRITE;
/*!40000 ALTER TABLE `quest` DISABLE KEYS */;
INSERT INTO `quest` VALUES (0,0,500,1000),(1,1,10,10);
/*!40000 ALTER TABLE `quest` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_require_finish_item`
--

DROP TABLE IF EXISTS `quest_require_finish_item`;
CREATE TABLE `quest_require_finish_item` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  `amount` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`item_template_id`),
  KEY `item_template_id` (`item_template_id`),
  CONSTRAINT `quest_require_finish_item_ibfk_1` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_require_finish_item_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_require_finish_item`
--

LOCK TABLES `quest_require_finish_item` WRITE;
/*!40000 ALTER TABLE `quest_require_finish_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `quest_require_finish_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_require_finish_quest`
--

DROP TABLE IF EXISTS `quest_require_finish_quest`;
CREATE TABLE `quest_require_finish_quest` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `req_quest_id` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`req_quest_id`),
  KEY `req_quest_id` (`req_quest_id`),
  CONSTRAINT `quest_require_finish_quest_ibfk_1` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_require_finish_quest_ibfk_2` FOREIGN KEY (`req_quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_require_finish_quest`
--

LOCK TABLES `quest_require_finish_quest` WRITE;
/*!40000 ALTER TABLE `quest_require_finish_quest` DISABLE KEYS */;
/*!40000 ALTER TABLE `quest_require_finish_quest` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_require_kill`
--

DROP TABLE IF EXISTS `quest_require_kill`;
CREATE TABLE `quest_require_kill` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `character_template_id` smallint(5) unsigned NOT NULL,
  `amount` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`character_template_id`),
  KEY `character_template_id` (`character_template_id`),
  CONSTRAINT `quest_require_kill_ibfk_1` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_require_kill_ibfk_2` FOREIGN KEY (`character_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_require_kill`
--

LOCK TABLES `quest_require_kill` WRITE;
/*!40000 ALTER TABLE `quest_require_kill` DISABLE KEYS */;
INSERT INTO `quest_require_kill` VALUES (0,1,5);
/*!40000 ALTER TABLE `quest_require_kill` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_require_start_item`
--

DROP TABLE IF EXISTS `quest_require_start_item`;
CREATE TABLE `quest_require_start_item` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  `amount` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`item_template_id`),
  KEY `item_template_id` (`item_template_id`),
  CONSTRAINT `quest_require_start_item_ibfk_1` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_require_start_item_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_require_start_item`
--

LOCK TABLES `quest_require_start_item` WRITE;
/*!40000 ALTER TABLE `quest_require_start_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `quest_require_start_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_require_start_quest`
--

DROP TABLE IF EXISTS `quest_require_start_quest`;
CREATE TABLE `quest_require_start_quest` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `req_quest_id` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`req_quest_id`),
  KEY `req_quest_id` (`req_quest_id`),
  CONSTRAINT `quest_require_start_quest_ibfk_1` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_require_start_quest_ibfk_2` FOREIGN KEY (`req_quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_require_start_quest`
--

LOCK TABLES `quest_require_start_quest` WRITE;
/*!40000 ALTER TABLE `quest_require_start_quest` DISABLE KEYS */;
/*!40000 ALTER TABLE `quest_require_start_quest` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_reward_item`
--

DROP TABLE IF EXISTS `quest_reward_item`;
CREATE TABLE `quest_reward_item` (
  `quest_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  `amount` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`quest_id`,`item_template_id`),
  KEY `item_template_id` (`item_template_id`),
  CONSTRAINT `quest_reward_item_ibfk_3` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `quest_reward_item_ibfk_4` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `quest_reward_item`
--

LOCK TABLES `quest_reward_item` WRITE;
/*!40000 ALTER TABLE `quest_reward_item` DISABLE KEYS */;
INSERT INTO `quest_reward_item` VALUES (0,3,1);
/*!40000 ALTER TABLE `quest_reward_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `server_setting`
--

DROP TABLE IF EXISTS `server_setting`;
CREATE TABLE `server_setting` (
  `motd` varchar(250) NOT NULL DEFAULT '' COMMENT 'The message of the day.'
) TYPE=InnoDB;

--
-- Dumping data for table `server_setting`
--

LOCK TABLES `server_setting` WRITE;
/*!40000 ALTER TABLE `server_setting` DISABLE KEYS */;
INSERT INTO `server_setting` VALUES ('Welcome to NetGore! Use the arrow keys to move, control to attack, alt to talk to NPCs and use world entities, and space to pick up items.');
/*!40000 ALTER TABLE `server_setting` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `server_time`
--

DROP TABLE IF EXISTS `server_time`;
CREATE TABLE `server_time` (
  `server_time` datetime NOT NULL,
  PRIMARY KEY (`server_time`)
) TYPE=InnoDB;

--
-- Dumping data for table `server_time`
--

LOCK TABLES `server_time` WRITE;
/*!40000 ALTER TABLE `server_time` DISABLE KEYS */;
INSERT INTO `server_time` VALUES ('2010-06-21 12:43:56');
/*!40000 ALTER TABLE `server_time` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shop`
--

DROP TABLE IF EXISTS `shop`;
CREATE TABLE `shop` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(60) NOT NULL,
  `can_buy` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) TYPE=InnoDB;

--
-- Dumping data for table `shop`
--

LOCK TABLES `shop` WRITE;
/*!40000 ALTER TABLE `shop` DISABLE KEYS */;
INSERT INTO `shop` VALUES (0,'Test Shop',1),(1,'Vending Machine',0);
/*!40000 ALTER TABLE `shop` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shop_item`
--

DROP TABLE IF EXISTS `shop_item`;
CREATE TABLE `shop_item` (
  `shop_id` smallint(5) unsigned NOT NULL,
  `item_template_id` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`shop_id`,`item_template_id`),
  KEY `item_template_id` (`item_template_id`),
  CONSTRAINT `shop_item_ibfk_1` FOREIGN KEY (`shop_id`) REFERENCES `shop` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `shop_item_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `shop_item`
--

LOCK TABLES `shop_item` WRITE;
/*!40000 ALTER TABLE `shop_item` DISABLE KEYS */;
INSERT INTO `shop_item` VALUES (0,1),(1,1),(0,2),(1,2),(0,3),(0,4),(0,5),(0,6),(0,7);
/*!40000 ALTER TABLE `shop_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary table structure for view `user_character`
--

DROP TABLE IF EXISTS `user_character`;
/*!50001 DROP VIEW IF EXISTS `user_character`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `user_character` (
  `id` int(11),
  `account_id` int(11),
  `character_template_id` smallint(5) unsigned,
  `name` varchar(30),
  `permissions` tinyint(3) unsigned,
  `shop_id` smallint(5) unsigned,
  `chat_dialog` smallint(5) unsigned,
  `ai_id` smallint(5) unsigned,
  `load_map_id` smallint(5) unsigned,
  `load_x` smallint(5) unsigned,
  `load_y` smallint(5) unsigned,
  `respawn_map_id` smallint(5) unsigned,
  `respawn_x` float,
  `respawn_y` float,
  `body_id` smallint(5) unsigned,
  `move_speed` smallint(5) unsigned,
  `cash` int(11),
  `level` tinyint(3) unsigned,
  `exp` int(11),
  `statpoints` int(11),
  `hp` smallint(6),
  `mp` smallint(6),
  `stat_maxhp` smallint(6),
  `stat_maxmp` smallint(6),
  `stat_minhit` smallint(6),
  `stat_maxhit` smallint(6),
  `stat_defence` smallint(6),
  `stat_agi` smallint(6),
  `stat_int` smallint(6),
  `stat_str` smallint(6)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `world_stats_guild_user_change`
--

DROP TABLE IF EXISTS `world_stats_guild_user_change`;
CREATE TABLE `world_stats_guild_user_change` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user who changed the guild they are part of.',
  `guild_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the guild, or null if the user left a guild.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `guild_id` (`guild_id`),
  CONSTRAINT `world_stats_guild_user_change_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_guild_user_change_ibfk_2` FOREIGN KEY (`guild_id`) REFERENCES `guild` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_guild_user_change`
--

LOCK TABLES `world_stats_guild_user_change` WRITE;
/*!40000 ALTER TABLE `world_stats_guild_user_change` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_guild_user_change` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_network`
--

DROP TABLE IF EXISTS `world_stats_network`;
CREATE TABLE `world_stats_network` (
  `when` timestamp NOT NULL COMMENT 'When these network stats were logged. The values correspond to a time period defined in the WorldStatsTracker constructor. This timestamp marks the end of this period of time. So all stats correspond to the time frame range: [when - rate, when].',
  `tcp_recv` mediumint(8) unsigned NOT NULL COMMENT 'The number of bytes that have been received over the TCP channel.',
  `tcp_recvs` mediumint(8) unsigned NOT NULL COMMENT 'The number of messages that have been received over the TCP channel.',
  `tcp_sent` mediumint(8) unsigned NOT NULL COMMENT 'The number of bytes that have been sent over the TCP channel.',
  `tcp_sends` mediumint(8) unsigned NOT NULL COMMENT 'The number of messages that have been sent over the TCP channel.',
  `udp_recv` mediumint(8) unsigned NOT NULL COMMENT 'The number of bytes that have been received over the UDP channel.',
  `udp_recvs` mediumint(8) unsigned NOT NULL COMMENT 'The number of messages that have been received over the UDP channel.',
  `udp_sent` mediumint(8) unsigned NOT NULL COMMENT 'The number of bytes that have been sent over the UDP channel.',
  `udp_sends` mediumint(8) unsigned NOT NULL COMMENT 'The number of messages that have been sent over the UDP channel.',
  `connections` mediumint(8) unsigned NOT NULL,
  PRIMARY KEY (`when`)
) TYPE=MyISAM;

--
-- Dumping data for table `world_stats_network`
--

LOCK TABLES `world_stats_network` WRITE;
/*!40000 ALTER TABLE `world_stats_network` DISABLE KEYS */;
INSERT INTO `world_stats_network` VALUES ('2010-06-04 05:50:54',0,0,0,0,0,0,0,0,0),('2010-06-08 00:58:08',0,0,0,0,0,0,0,0,0),('2010-06-08 01:30:03',0,0,0,0,0,0,0,0,0),('2010-06-08 01:32:45',0,0,0,0,0,0,0,0,0),('2010-06-08 01:35:53',0,0,0,0,0,0,0,0,0),('2010-06-08 01:37:03',0,0,0,0,0,0,0,0,0),('2010-06-08 01:38:14',0,0,0,0,0,0,0,0,0),('2010-06-08 01:39:29',0,0,0,0,0,0,0,0,0),('2010-06-08 01:41:10',0,0,0,0,0,0,0,0,0),('2010-06-08 01:42:10',0,0,0,0,0,0,0,0,0),('2010-06-08 01:44:11',0,0,0,0,0,0,0,0,0),('2010-06-08 01:45:11',0,0,0,0,0,0,0,0,0),('2010-06-08 01:46:11',0,0,0,0,0,0,0,0,0),('2010-06-11 09:14:26',0,0,0,0,0,0,0,0,0),('2010-06-11 09:15:26',0,0,0,0,0,0,0,0,0),('2010-06-16 02:01:10',0,0,0,0,0,0,0,0,0),('2010-06-16 02:02:09',0,0,0,0,0,0,0,0,0),('2010-06-16 02:03:37',0,0,0,0,0,0,0,0,0),('2010-06-16 02:04:37',0,0,0,0,0,0,0,0,0),('2010-06-16 02:05:26',0,0,0,0,0,0,0,0,0),('2010-06-17 18:45:16',0,0,0,0,0,0,0,0,0),('2010-06-17 18:46:16',0,0,0,0,0,0,0,0,0),('2010-06-17 18:58:08',0,0,0,0,0,0,0,0,0),('2010-06-17 18:59:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:00:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:01:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:02:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:03:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:04:16',0,0,0,0,0,0,0,0,0),('2010-06-17 19:05:24',0,0,0,0,0,0,0,0,0),('2010-06-17 19:06:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:07:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:10:46',0,0,0,0,0,0,0,0,0),('2010-06-17 19:11:08',0,0,0,0,0,0,0,0,0),('2010-06-17 19:12:10',0,0,0,0,0,0,0,0,0),('2010-06-17 19:13:10',0,0,0,0,0,0,0,0,0),('2010-06-17 19:14:10',0,0,0,0,0,0,0,0,0),('2010-06-17 19:20:50',0,0,0,0,0,0,0,0,0),('2010-06-17 19:21:50',0,0,0,0,0,0,0,0,0),('2010-06-17 19:22:50',0,0,0,0,0,0,0,0,0),('2010-06-17 19:23:50',0,0,0,0,0,0,0,0,0),('2010-06-17 19:28:00',0,0,0,0,0,0,0,0,0),('2010-06-17 19:32:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:33:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:34:53',0,0,0,0,0,0,0,0,0),('2010-06-17 19:35:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:36:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:37:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:38:51',0,0,0,0,0,0,0,0,0),('2010-06-17 19:39:51',0,0,0,0,0,0,0,0,0),('2010-06-17 20:35:24',0,0,0,0,0,0,0,0,0),('2010-06-17 20:36:24',0,0,0,0,0,0,0,0,0),('2010-06-17 22:39:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:40:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:41:13',0,0,0,0,0,0,0,0,0),('2010-06-17 22:42:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:43:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:44:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:45:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:46:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:47:09',0,0,0,0,0,0,0,0,0),('2010-06-17 22:48:07',0,0,0,0,0,0,0,0,0),('2010-06-17 22:49:07',0,0,0,0,0,0,0,0,0),('2010-06-17 22:50:07',0,0,0,0,0,0,0,0,0),('2010-06-18 03:19:44',0,0,0,0,0,0,0,0,0),('2010-06-18 03:25:46',0,0,0,0,0,0,0,0,0),('2010-06-18 03:26:46',0,0,0,0,0,0,0,0,0),('2010-06-18 03:33:17',0,0,0,0,0,0,0,0,0),('2010-06-18 03:34:19',0,0,0,0,0,0,0,0,0),('2010-06-18 03:35:23',0,0,0,0,0,0,0,0,0),('2010-06-18 03:50:20',0,0,0,0,0,0,0,0,0),('2010-06-18 03:51:53',0,0,0,0,0,0,0,0,0),('2010-06-18 03:52:09',0,0,0,0,0,0,0,0,0),('2010-06-18 03:53:09',0,0,0,0,0,0,0,0,0),('2010-06-18 03:54:09',0,0,0,0,0,0,0,0,0),('2010-06-18 03:56:32',0,0,0,0,0,0,0,0,0),('2010-06-18 04:01:02',0,0,0,0,0,0,0,0,0),('2010-06-21 18:47:40',0,0,0,0,0,0,0,0,0),('2010-06-21 19:31:46',0,0,0,0,0,0,0,0,0),('2010-06-21 19:34:40',0,0,0,0,0,0,0,0,0),('2010-06-21 19:37:15',0,0,0,0,0,0,0,0,0),('2010-06-21 19:38:15',0,0,0,0,0,0,0,0,0),('2010-06-21 19:43:22',0,0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `world_stats_network` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_npc_kill_user`
--

DROP TABLE IF EXISTS `world_stats_npc_kill_user`;
CREATE TABLE `world_stats_npc_kill_user` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user.',
  `npc_template_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The template ID of the NPC. Only valid when the NPC has a template ID set.',
  `user_level` tinyint(3) unsigned NOT NULL COMMENT 'The level of the user was when this event took place.',
  `user_x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place.',
  `user_y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place.',
  `npc_x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the NPC when this event took place. Only valid when the map_id is not null.',
  `npc_y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the NPC when this event took place. Only valid when the map_id is not null.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `npc_template_id` (`npc_template_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_npc_kill_user_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_npc_kill_user_ibfk_2` FOREIGN KEY (`npc_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_npc_kill_user_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_npc_kill_user`
--

LOCK TABLES `world_stats_npc_kill_user` WRITE;
/*!40000 ALTER TABLE `world_stats_npc_kill_user` DISABLE KEYS */;
INSERT INTO `world_stats_npc_kill_user` VALUES (1,1,31,765,45,833,658,1,'2010-06-08 01:42:21'),(1,1,30,765,45,285,658,1,'2010-06-21 19:43:45');
/*!40000 ALTER TABLE `world_stats_npc_kill_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_quest_accept`
--

DROP TABLE IF EXISTS `world_stats_quest_accept`;
CREATE TABLE `world_stats_quest_accept` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user that accepted the quest.',
  `quest_id` smallint(5) unsigned NOT NULL COMMENT 'The quest that was accepted.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `quest_id` (`quest_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_quest_accept_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_accept_ibfk_2` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_accept_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB ROW_FORMAT=COMPACT;

--
-- Dumping data for table `world_stats_quest_accept`
--

LOCK TABLES `world_stats_quest_accept` WRITE;
/*!40000 ALTER TABLE `world_stats_quest_accept` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_quest_accept` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_quest_cancel`
--

DROP TABLE IF EXISTS `world_stats_quest_cancel`;
CREATE TABLE `world_stats_quest_cancel` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user that canceled the quest.',
  `quest_id` smallint(5) unsigned NOT NULL COMMENT 'The quest that was canceled.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `quest_id` (`quest_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_quest_cancel_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_cancel_ibfk_2` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_cancel_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB ROW_FORMAT=COMPACT;

--
-- Dumping data for table `world_stats_quest_cancel`
--

LOCK TABLES `world_stats_quest_cancel` WRITE;
/*!40000 ALTER TABLE `world_stats_quest_cancel` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_quest_cancel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_quest_complete`
--

DROP TABLE IF EXISTS `world_stats_quest_complete`;
CREATE TABLE `world_stats_quest_complete` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user that completed the quest.',
  `quest_id` smallint(5) unsigned NOT NULL COMMENT 'The quest that was completed.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `quest_id` (`quest_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_quest_complete_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_complete_ibfk_2` FOREIGN KEY (`quest_id`) REFERENCES `quest` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_quest_complete_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_quest_complete`
--

LOCK TABLES `world_stats_quest_complete` WRITE;
/*!40000 ALTER TABLE `world_stats_quest_complete` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_quest_complete` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_user_consume_item`
--

DROP TABLE IF EXISTS `world_stats_user_consume_item`;
CREATE TABLE `world_stats_user_consume_item` (
  `user_id` int(11) NOT NULL COMMENT 'The user that this event is related to.',
  `item_template_id` smallint(5) unsigned NOT NULL COMMENT 'The template ID of the item that was consumed. Only valid when the item has a set template ID.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The map the user was on when this event took place.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `item_template_id` (`item_template_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_user_consume_item_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_consume_item_ibfk_2` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_consume_item_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_user_consume_item`
--

LOCK TABLES `world_stats_user_consume_item` WRITE;
/*!40000 ALTER TABLE `world_stats_user_consume_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_user_consume_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_user_kill_npc`
--

DROP TABLE IF EXISTS `world_stats_user_kill_npc`;
CREATE TABLE `world_stats_user_kill_npc` (
  `user_id` int(11) NOT NULL COMMENT 'The ID of the user.',
  `npc_template_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The template ID of the NPC. Only valid when the NPC has a template ID set.',
  `user_level` tinyint(3) unsigned NOT NULL COMMENT 'The level of the user was when this event took place.',
  `user_x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `user_y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `npc_x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the NPC when this event took place. Only valid when the map_id is not null.',
  `npc_y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the NPC when this event took place. Only valid when the map_id is not null.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `user_id` (`user_id`),
  KEY `npc_template_id` (`npc_template_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_user_kill_npc_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_kill_npc_ibfk_2` FOREIGN KEY (`npc_template_id`) REFERENCES `character_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_kill_npc_ibfk_3` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_user_kill_npc`
--

LOCK TABLES `world_stats_user_kill_npc` WRITE;
/*!40000 ALTER TABLE `world_stats_user_kill_npc` DISABLE KEYS */;
INSERT INTO `world_stats_user_kill_npc` VALUES (1,1,30,392,658,0,0,1,'2010-06-08 01:41:37'),(1,1,30,446,658,0,0,1,'2010-06-08 01:41:43'),(1,1,30,417,658,0,0,1,'2010-06-08 01:42:10'),(1,1,30,824,658,0,0,1,'2010-06-08 01:42:17'),(1,1,30,689,658,0,0,1,'2010-06-08 01:44:23'),(1,1,30,235,658,0,0,1,'2010-06-08 01:44:26'),(1,1,30,246,500,0,0,1,'2010-06-08 01:44:52'),(1,1,30,246,500,0,0,1,'2010-06-08 01:45:11'),(1,1,31,219,500,0,0,1,'2010-06-08 01:45:42'),(1,1,31,219,500,0,0,1,'2010-06-08 01:45:47'),(1,1,30,212,500,0,0,1,'2010-06-17 18:58:42'),(1,1,30,212,500,0,0,1,'2010-06-17 18:58:44'),(1,1,30,212,500,0,0,1,'2010-06-17 19:01:00'),(1,1,30,212,500,0,0,1,'2010-06-17 19:01:17'),(1,1,31,212,500,0,0,1,'2010-06-17 19:04:21'),(1,1,31,212,500,0,0,1,'2010-06-17 19:04:39'),(1,1,30,205,500,0,0,1,'2010-06-17 19:11:44'),(1,1,30,238,500,0,0,1,'2010-06-17 19:12:10'),(1,1,30,707,658,0,0,1,'2010-06-17 19:33:02'),(1,1,30,534,658,0,0,1,'2010-06-17 19:33:06'),(1,1,30,135,658,0,0,1,'2010-06-17 19:33:12'),(1,1,30,218,500,0,0,1,'2010-06-17 19:33:31'),(1,1,31,225,500,0,0,1,'2010-06-17 19:35:16'),(1,1,31,225,500,0,0,1,'2010-06-17 19:36:10'),(1,1,31,225,500,0,0,1,'2010-06-17 19:36:14'),(1,1,31,225,500,0,0,1,'2010-06-17 19:36:23'),(1,1,31,223,500,0,0,1,'2010-06-17 19:37:17'),(1,1,31,223,500,0,0,1,'2010-06-17 19:38:12'),(1,1,32,223,500,0,0,1,'2010-06-17 19:38:37'),(1,1,32,223,500,0,0,1,'2010-06-17 19:38:40'),(1,1,30,210,500,0,0,1,'2010-06-17 20:36:24'),(1,1,30,397,539,0,0,1,'2010-06-17 22:39:23'),(1,1,30,204,500,0,0,1,'2010-06-17 22:39:30'),(1,1,30,204,500,0,0,1,'2010-06-17 22:41:13'),(1,1,30,204,500,0,0,1,'2010-06-17 22:41:32'),(1,1,31,204,500,0,0,1,'2010-06-17 22:41:43'),(1,1,31,247,500,0,0,1,'2010-06-17 22:41:56'),(1,1,31,1294,532,0,0,1,'2010-06-17 22:44:17'),(1,1,31,1294,532,0,0,1,'2010-06-17 22:44:31'),(1,1,31,1294,532,0,0,1,'2010-06-17 22:44:35'),(1,1,31,1320,532,0,0,1,'2010-06-17 22:45:03'),(1,1,32,1320,532,0,0,1,'2010-06-17 22:45:09'),(1,1,32,1320,532,0,0,1,'2010-06-17 22:46:52'),(1,1,32,1320,532,0,0,1,'2010-06-17 22:47:04'),(1,1,32,1320,532,0,0,1,'2010-06-17 22:47:08'),(1,1,30,218,500,0,0,1,'2010-06-17 22:48:53'),(1,1,30,218,500,0,0,1,'2010-06-17 22:48:55'),(1,1,30,218,500,0,0,1,'2010-06-17 22:49:00'),(1,1,30,218,500,0,0,1,'2010-06-17 22:49:07'),(1,1,31,218,500,0,0,1,'2010-06-17 22:49:09'),(1,1,31,1298,532,0,0,1,'2010-06-17 22:49:20'),(1,1,31,1298,532,0,0,1,'2010-06-17 22:49:23'),(1,1,30,473,658,0,0,1,'2010-06-18 03:33:24'),(1,1,30,107,543,0,0,1,'2010-06-18 03:35:31'),(1,1,30,234,500,0,0,1,'2010-06-18 03:36:00'),(1,1,30,73,534,0,0,1,'2010-06-21 19:34:53'),(1,1,30,369,658,0,0,1,'2010-06-21 19:37:23'),(1,1,30,473,658,0,0,1,'2010-06-21 19:43:40'),(1,1,30,480,658,0,0,1,'2010-06-21 19:43:52');
/*!40000 ALTER TABLE `world_stats_user_kill_npc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_user_level`
--

DROP TABLE IF EXISTS `world_stats_user_level`;
CREATE TABLE `world_stats_user_level` (
  `character_id` int(11) NOT NULL COMMENT 'The ID of the character that leveled up.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map this event took place on.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map x coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map y coordinate of the user when this event took place. Only valid when the map_id is not null.',
  `level` tinyint(3) unsigned NOT NULL COMMENT 'The level that the character leveled up to (their new level).',
  `when` timestamp NOT NULL COMMENT 'When this event took place.'
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_user_level`
--

LOCK TABLES `world_stats_user_level` WRITE;
/*!40000 ALTER TABLE `world_stats_user_level` DISABLE KEYS */;
INSERT INTO `world_stats_user_level` VALUES (1,1,824,658,31,'2010-06-08 01:42:17'),(1,1,246,500,31,'2010-06-08 01:45:11'),(1,1,212,500,31,'2010-06-17 19:01:17'),(1,1,218,500,31,'2010-06-17 19:33:31'),(1,1,223,500,32,'2010-06-17 19:38:12'),(1,1,204,500,31,'2010-06-17 22:41:32'),(1,1,1320,532,32,'2010-06-17 22:45:03'),(1,1,218,500,31,'2010-06-17 22:49:07');
/*!40000 ALTER TABLE `world_stats_user_level` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `world_stats_user_shopping`
--

DROP TABLE IF EXISTS `world_stats_user_shopping`;
CREATE TABLE `world_stats_user_shopping` (
  `shop_id` smallint(5) unsigned NOT NULL COMMENT 'The ID of the shop the event took place at.',
  `character_id` int(11) NOT NULL COMMENT 'The ID of the character that performed this transaction with the shop.',
  `item_template_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the item template that the event relates to. Only valid when the item involved has a set item template ID.',
  `map_id` smallint(5) unsigned DEFAULT NULL COMMENT 'The ID of the map the event took place on.',
  `x` smallint(5) unsigned NOT NULL COMMENT 'The map X coordinate of the shopper when this event took place. Only valid when the map_id is not null.',
  `y` smallint(5) unsigned NOT NULL COMMENT 'The map Y coordinate of the shopper when this event took place. Only valid when the map_id is not null.',
  `cost` int(11) NOT NULL COMMENT 'The amount of money that was involved in this transaction (how much the shopper sold the items for, or how much they bought the items for). ',
  `amount` tinyint(3) unsigned NOT NULL COMMENT 'The number of items involved in the transaction. Should always be greater than 0, and should only be greater for 1 for items that can stack.',
  `sale_type` tinyint(4) NOT NULL COMMENT 'Whether the shop sold to the user, or vise versa. If 0, the shop sold an item to the shopper. If non-zero, the shopper sold an item to a shop.',
  `when` timestamp NOT NULL COMMENT 'When this event took place.',
  KEY `shop_id` (`shop_id`),
  KEY `character_id` (`character_id`),
  KEY `item_template_id` (`item_template_id`),
  KEY `map_id` (`map_id`),
  CONSTRAINT `world_stats_user_shopping_ibfk_1` FOREIGN KEY (`shop_id`) REFERENCES `shop` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_shopping_ibfk_2` FOREIGN KEY (`character_id`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_shopping_ibfk_3` FOREIGN KEY (`item_template_id`) REFERENCES `item_template` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `world_stats_user_shopping_ibfk_4` FOREIGN KEY (`map_id`) REFERENCES `map` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) TYPE=InnoDB;

--
-- Dumping data for table `world_stats_user_shopping`
--

LOCK TABLES `world_stats_user_shopping` WRITE;
/*!40000 ALTER TABLE `world_stats_user_shopping` DISABLE KEYS */;
/*!40000 ALTER TABLE `world_stats_user_shopping` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'demogame'
--
/*!50003 DROP FUNCTION IF EXISTS `CreateUserOnAccount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 FUNCTION `CreateUserOnAccount`(accountName VARCHAR(50), characterName VARCHAR(30), characterID INT) RETURNS varchar(100) CHARSET latin1
BEGIN
		
		DECLARE character_count INT DEFAULT 0;
		DECLARE max_character_count INT DEFAULT 3;
		DECLARE is_id_free INT DEFAULT 0;
		DECLARE is_name_free INT DEFAULT 0;
		DECLARE errorMsg VARCHAR(100) DEFAULT "";
		DECLARE accountID INT DEFAULT NULL;

		SELECT `id` INTO accountID FROM `account` WHERE `name` = accountName;

		IF ISNULL(accountID) THEN
			SET errorMsg = "Account with the specified name does not exist.";
		ELSE
			SELECT COUNT(*) INTO character_count FROM `character` WHERE `account_id` = accountID;
			SELECT `max_characters_per_account` INTO max_character_count FROM `game_constant`;

			IF character_count > max_character_count THEN
				SET errorMsg = "No free character slots available in the account.";
			ELSE
				SELECT COUNT(*) INTO is_id_free FROM `character` WHERE `id` = characterID;
				
				IF is_id_free > 0 THEN
					SET errorMsg = "The specified CharacterID is not available for use.";
				ELSE
					SELECT COUNT(*) INTO is_name_free FROM `user_character` WHERE `name` = characterName;
						
					IF is_name_free > 0 THEN
						SET errorMsg = "The specified character name is not available for use.";
					ELSE
						INSERT INTO `character` SET `id` = characterID, `name`	= characterName, `account_id`= 	accountID;
					END IF;
				END IF;
			END IF;
		END IF;
				
		RETURN errorMsg;
  
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Rebuild_Views` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `Rebuild_Views`()
BEGIN
	
	CALL Rebuild_View_NPC_Character();
	CALL Rebuild_View_User_Character();
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Rebuild_View_NPC_Character` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = latin1 */ ;
/*!50003 SET character_set_results = latin1 */ ;
/*!50003 SET collation_connection  = latin1_swedish_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `Rebuild_View_NPC_Character`()
BEGIN
	
	DROP VIEW IF EXISTS `npc_character`;
	CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `npc_character` AS SELECT *FROM `character` WHERE `account_id` IS NULL;
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Rebuild_View_User_Character` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`%`*/ /*!50003 PROCEDURE `Rebuild_View_User_Character`()
BEGIN
	
	DROP VIEW IF EXISTS `user_character`;
	CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `user_character` AS SELECT * FROM `character` WHERE `account_id` IS NOT NULL;
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `npc_character`
--

/*!50001 DROP TABLE IF EXISTS `npc_character`*/;
/*!50001 DROP VIEW IF EXISTS `npc_character`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = latin1 */;
/*!50001 SET character_set_results     = latin1 */;
/*!50001 SET collation_connection      = latin1_swedish_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `npc_character` AS select `character`.`id` AS `id`,`character`.`account_id` AS `account_id`,`character`.`character_template_id` AS `character_template_id`,`character`.`name` AS `name`,`character`.`permissions` AS `permissions`,`character`.`shop_id` AS `shop_id`,`character`.`chat_dialog` AS `chat_dialog`,`character`.`ai_id` AS `ai_id`,`character`.`load_map_id` AS `load_map_id`,`character`.`load_x` AS `load_x`,`character`.`load_y` AS `load_y`,`character`.`respawn_map_id` AS `respawn_map_id`,`character`.`respawn_x` AS `respawn_x`,`character`.`respawn_y` AS `respawn_y`,`character`.`body_id` AS `body_id`,`character`.`move_speed` AS `move_speed`,`character`.`cash` AS `cash`,`character`.`level` AS `level`,`character`.`exp` AS `exp`,`character`.`statpoints` AS `statpoints`,`character`.`hp` AS `hp`,`character`.`mp` AS `mp`,`character`.`stat_maxhp` AS `stat_maxhp`,`character`.`stat_maxmp` AS `stat_maxmp`,`character`.`stat_minhit` AS `stat_minhit`,`character`.`stat_maxhit` AS `stat_maxhit`,`character`.`stat_defence` AS `stat_defence`,`character`.`stat_agi` AS `stat_agi`,`character`.`stat_int` AS `stat_int`,`character`.`stat_str` AS `stat_str` from `character` where isnull(`character`.`account_id`) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `user_character`
--

/*!50001 DROP TABLE IF EXISTS `user_character`*/;
/*!50001 DROP VIEW IF EXISTS `user_character`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `user_character` AS select `character`.`id` AS `id`,`character`.`account_id` AS `account_id`,`character`.`character_template_id` AS `character_template_id`,`character`.`name` AS `name`,`character`.`permissions` AS `permissions`,`character`.`shop_id` AS `shop_id`,`character`.`chat_dialog` AS `chat_dialog`,`character`.`ai_id` AS `ai_id`,`character`.`load_map_id` AS `load_map_id`,`character`.`load_x` AS `load_x`,`character`.`load_y` AS `load_y`,`character`.`respawn_map_id` AS `respawn_map_id`,`character`.`respawn_x` AS `respawn_x`,`character`.`respawn_y` AS `respawn_y`,`character`.`body_id` AS `body_id`,`character`.`move_speed` AS `move_speed`,`character`.`cash` AS `cash`,`character`.`level` AS `level`,`character`.`exp` AS `exp`,`character`.`statpoints` AS `statpoints`,`character`.`hp` AS `hp`,`character`.`mp` AS `mp`,`character`.`stat_maxhp` AS `stat_maxhp`,`character`.`stat_maxmp` AS `stat_maxmp`,`character`.`stat_minhit` AS `stat_minhit`,`character`.`stat_maxhit` AS `stat_maxhit`,`character`.`stat_defence` AS `stat_defence`,`character`.`stat_agi` AS `stat_agi`,`character`.`stat_int` AS `stat_int`,`character`.`stat_str` AS `stat_str` from `character` where (`character`.`account_id` is not null) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2010-06-21 12:47:26

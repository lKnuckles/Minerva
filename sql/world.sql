-- phpMyAdmin SQL Dump
-- version 4.5.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: 2016 m. Bal 30 d. 18:49
-- Server version: 10.1.9-MariaDB
-- PHP Version: 5.6.15

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `world`
--

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters`
--

CREATE TABLE `characters` (
  `id` int(11) NOT NULL,
  `account` int(11) NOT NULL,
  `slot` tinyint(4) UNSIGNED NOT NULL,
  `name` varchar(32) NOT NULL,
  `level` int(11) UNSIGNED NOT NULL DEFAULT '1',
  `class` tinyint(4) UNSIGNED NOT NULL,
  `gender` tinyint(1) NOT NULL,
  `face` tinyint(4) UNSIGNED NOT NULL,
  `hair` tinyint(4) UNSIGNED NOT NULL,
  `colour` tinyint(4) UNSIGNED NOT NULL,
  `map` tinyint(4) UNSIGNED NOT NULL,
  `x` tinyint(4) UNSIGNED NOT NULL,
  `y` tinyint(4) UNSIGNED NOT NULL,
  `created` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters_equipment`
--

CREATE TABLE `characters_equipment` (
  `charId` int(11) NOT NULL,
  `head` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `body` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `hands` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `feet` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `lefthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `righthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `neck` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger1` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger2` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger3` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger4` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `back` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `card` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters_items`
--

CREATE TABLE `characters_items` (
  `id` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `item` binary(15) NOT NULL,
  `amount` smallint(5) UNSIGNED NOT NULL,
  `slot` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters_quickslots`
--

CREATE TABLE `characters_quickslots` (
  `id` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `skill` tinyint(3) UNSIGNED NOT NULL,
  `slot` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters_skills`
--

CREATE TABLE `characters_skills` (
  `id` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `skill` smallint(5) UNSIGNED NOT NULL,
  `level` tinyint(3) UNSIGNED NOT NULL,
  `slot` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `characters_stats`
--

CREATE TABLE `characters_stats` (
  `charId` int(11) NOT NULL,
  `curhp` smallint(5) UNSIGNED NOT NULL,
  `maxhp` smallint(5) UNSIGNED NOT NULL,
  `curmp` smallint(5) UNSIGNED NOT NULL,
  `maxmp` smallint(5) UNSIGNED NOT NULL,
  `cursp` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `maxsp` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `exp` bigint(20) UNSIGNED NOT NULL DEFAULT '0',
  `str_stat` int(11) UNSIGNED NOT NULL,
  `int_stat` int(11) UNSIGNED NOT NULL,
  `dex_stat` int(11) UNSIGNED NOT NULL,
  `pnt_stat` int(11) UNSIGNED NOT NULL DEFAULT '0',
  `honour` int(11) UNSIGNED NOT NULL DEFAULT '1',
  `rank` int(11) UNSIGNED NOT NULL DEFAULT '1',
  `swordrank` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `swordxp` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `swordpoints` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `magicrank` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `magicxp` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `magicpoints` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `alz` bigint(20) UNSIGNED NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Sukurta duomenų struktūra lentelei `slotorder`
--

CREATE TABLE `slotorder` (
  `id` int(11) NOT NULL,
  `slotorder` int(11) NOT NULL DEFAULT '1193046'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `characters`
--
ALTER TABLE `characters`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `characters_equipment`
--
ALTER TABLE `characters_equipment`
  ADD PRIMARY KEY (`charId`);

--
-- Indexes for table `characters_items`
--
ALTER TABLE `characters_items`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `characters_quickslots`
--
ALTER TABLE `characters_quickslots`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `characters_skills`
--
ALTER TABLE `characters_skills`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `characters_stats`
--
ALTER TABLE `characters_stats`
  ADD PRIMARY KEY (`charId`);

--
-- Indexes for table `slotorder`
--
ALTER TABLE `slotorder`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `characters_items`
--
ALTER TABLE `characters_items`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `characters_quickslots`
--
ALTER TABLE `characters_quickslots`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `characters_skills`
--
ALTER TABLE `characters_skills`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

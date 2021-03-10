create database Cooking;
use Cooking;


DROP TABLE IF EXISTS `Client` ; 
CREATE TABLE `Cooking`.`Client` (
`idClient` VARCHAR(20) NOT NULL, 
`prenom_C` VARCHAR(20) NOT NULL,
`nom_C` VARCHAR(20) NOT NULL,  
`num_tel` VARCHAR(20) NOT NULL, 
`num_compte` VARCHAR(20) NOT NULL,
`Adresse` VARCHAR(250) NOT NULL,
`Statut_CdR` BOOL NOT NULL, 
`nbr_recette_com_CdR` BIGINT NOT NULL, 
`solde_cook` BIGINT NOT NULL, 
`idCuisinier` VARCHAR(20), 
PRIMARY KEY (`idClient`)); 

DROP TABLE IF EXISTS `Produit` ; 
CREATE TABLE `Cooking`.`Produit` (
`idProduit` VARCHAR(20) NOT NULL, 
`nom_P` VARCHAR(20) NOT NULL, 
`categorie` VARCHAR(20) NOT NULL, 
`stock_actuel` BIGINT NOT NULL, 
`stock_min` BIGINT NOT NULL, 
`stock_max` BIGINT NOT NULL, 
PRIMARY KEY (`idProduit`));  

DROP TABLE IF EXISTS `Recette`; 
CREATE TABLE `Cooking`.`Recette`(
`idRecette` VARCHAR(20) NOT NULL, 
`nom_R` VARCHAR(20) NOT NULL, 
`type` VARCHAR(20) NOT NULL, 
`descriptif` VARCHAR(256) NOT NULL, 
`prix_vente` BIGINT NOT NULL, 
`nb_commande` BIGINT NOT NULL, 
`temps_conservation` BIGINT NOT NULL, 
`idClient`VARCHAR(20) NOT NULL,
PRIMARY KEY (`idRecette`));  

DROP TABLE IF EXISTS `Fournisseur`; 
CREATE TABLE `Cooking`.`Fournisseur` (
`idFournisseur` VARCHAR(20) NOT NULL, 
`nom_F` VARCHAR(20) NOT NULL, 
`num_tel_F` VARCHAR(20) NOT NULL, 
PRIMARY KEY (`idFournisseur`));  

DROP TABLE IF EXISTS `Cuisinier`; 
CREATE TABLE `Cooking`.`Cuisinier` (
`idCuisinier` VARCHAR(20) NOT NULL, 
`DateDebut` DATETIME NOT NULL, 
`idClient` VARCHAR(20) NOT NULL,
PRIMARY KEY (`idCuisinier`));  

DROP TABLE IF EXISTS `Commande` ; 
CREATE TABLE `Cooking`.`Commande` (
`idCommande` VARCHAR(20) NOT NULL, 
`prixCommande` BIGINT NOT NULL, 
`date_commande` DATETIME NOT NULL, 
`idClient` VARCHAR(20) NOT NULL,
PRIMARY KEY (`idCommande`));

DROP TABLE IF EXISTS `fournit_stock` ; 
CREATE TABLE `Cooking`.`fournit_stock` (
`idFournisseur` VARCHAR(20) NOT NULL, 
`idProduit` VARCHAR(20) NOT NULL, 
`ref_fournisseur` VARCHAR(20) NOT NULL, 
`date_ajout` DATETIME NOT NULL, 
PRIMARY KEY (`idFournisseur`, `idProduit`));  

DROP TABLE IF EXISTS `contient` ; 
CREATE TABLE `Cooking`.`contient` (
`idCommande` VARCHAR(20) NOT NULL, 
`idRecette` VARCHAR(20) NOT NULL, 
`quantite_commande` BIGINT NOT NULL, 
PRIMARY KEY (`idCommande`,  `idRecette`)) ; 

DROP TABLE IF EXISTS `listeIngredients` ; 
CREATE TABLE `Cooking`.`listeIngredients` (
`idProduit` VARCHAR(20) NOT NULL, 
`idRecette` VARCHAR(20) NOT NULL, 
`unite_quantite` BIGINT NOT NULL, 
PRIMARY KEY (`idProduit`,  `idRecette`));  

DROP TABLE IF EXISTS `prepare` ; 
CREATE TABLE `Cooking`.`prepare` (
`idCuisinier` VARCHAR(20) NOT NULL, 
`idRecette` VARCHAR(20) NOT NULL, 
`date_preparation` DATETIME NOT NULL, 
PRIMARY KEY (`idCuisinier`, `idRecette`));  

-- ALTER TABLE `Client` ADD CONSTRAINT FK_Client_idCuisinier FOREIGN KEY (`idCuisinier`) REFERENCES `Cuisinier` (`idCuisinier`); 
ALTER TABLE `Recette` ADD CONSTRAINT FK_Recette_idClient FOREIGN KEY (`idClient`) REFERENCES `Client` (`idClient`); 
ALTER TABLE `Cuisinier` ADD CONSTRAINT FK_Commande_idClient FOREIGN KEY (`idClient`) REFERENCES `Client` (`idClient`); 
ALTER TABLE `fournit_stock` ADD CONSTRAINT FK_idFournisseur FOREIGN KEY (`idFournisseur`) REFERENCES `Fournisseur` (`idFournisseur`); 
ALTER TABLE `fournit_stock` ADD CONSTRAINT FK_fournit_stock_idProduit FOREIGN KEY (`idProduit`) REFERENCES `Produit` (`idProduit`); 
ALTER TABLE `contient` ADD CONSTRAINT FK_contient_idCommande FOREIGN KEY (`idCommande`) REFERENCES `Commande` (`idCommande`); 
ALTER TABLE `contient` ADD CONSTRAINT FK_contient_idRecette FOREIGN KEY (`idRecette`) REFERENCES `Recette` (`idRecette`); 
ALTER TABLE `listeIngredients` ADD CONSTRAINT FK_listeIngredients_idProduit FOREIGN KEY (`idProduit`) REFERENCES `Produit` (`idProduit`); 
ALTER TABLE `listeIngredients` ADD CONSTRAINT FK_listeIngredients_idRecette FOREIGN KEY (`idRecette`) REFERENCES `Recette` (`idRecette`); 
ALTER TABLE `prepare` ADD CONSTRAINT FK_prepare_idCuisinier FOREIGN KEY (`idCuisinier`) REFERENCES `Cuisinier` (`idCuisinier`); 
ALTER TABLE `prepare` ADD CONSTRAINT FK_prepare_idRecette FOREIGN KEY (`idRecette`) REFERENCES `Recette` (`idRecette`);
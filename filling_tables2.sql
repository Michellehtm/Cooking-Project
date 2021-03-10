
use Cooking;

-- filling client
insert into client (`idClient`,`prenom_C`,`nom_C`,`num_tel`,`num_compte`,`Adresse`,`statut_CdR`,`nbr_recette_com_CdR`,`solde_cook`,`idCuisinier`) VALUES
('A0001','Marine', 'Dupont','0657489624','FR67548932','10 rue Hittorf 75010 Paris',true,9,50,0),
('A0002','Jean', 'Dupont','0657435624','FR89658932','23 rue des Ardennes 75019 Paris',true,6,90,0),
('A0003','Marie', 'Garcia','0784953046','FR75849305','55 rue du Chemin Vert 75011 Paris',true,0,0,0),
('A0004','James', 'Wilkes','0657489624','FR67548932','31 rue de Poitou 75003 Paris',true,9,50,0),
('A0005','Jean-Christophe', 'Zoc','0689432051','FR83450218','74 Rue des Saules 75018 Paris',true,20,100,0),
('A0006','Laurent', 'Philippe','0748232122','FR89450075','37 Rue Davy 75017 Paris',false,0,0,0),
('A0007','Jessica', 'Lopez','0648392019','FR89450075','45 Rue Cardinet 75017 Paris',true,8,10,0),
('A0008','Mikael', 'Joeffroy','0634529871','FR76392013','27 Rue de Grenelle 75007 Paris',false,0,0,0),
('A0009','Moussa', 'Lacrip','0657382910','FR56342356','3 Rue Blomet 75015 Paris',true,29,100,0),
('A0010','Renée', 'Dufourt','0784923421','FR57478999','21 Rue Pirandello 75013 Paris',false,0,0,0);

-- filling cuisinier
insert into cuisinier (`idCuisinier`,`DateDebut`,`idClient`) VALUES 
('C0001','2019-8-3','A0001'),
('C0002','2019-8-3','A0002'), 
('C0004','2020-2-1','A0004'),
('C0005','2019-11-14','A0005'), 
('C0007','2020-01-23','A0007'), 
('C0009','2019-12-30','A0009');

-- adding the idcuisinier to client 
UPDATE client SET idCuisinier = 'C0001' WHERE idClient='A0001';
UPDATE client SET idCuisinier = 'C0002' WHERE idClient='A0002';
UPDATE client SET idCuisinier = 'C0004' WHERE idClient='A0004';
UPDATE client SET idCuisinier = 'C0005' WHERE idClient='A0005';
UPDATE client SET idCuisinier = 'C0007' WHERE idClient='A0007';
UPDATE client SET idCuisinier = 'C0009' WHERE idClient='A0009';


-- filling profuit
insert into produit (`idProduit`,`nom_P`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`) VALUES
('P0001','Banane','BIO',10,4,30),
('P0002','Pomme','BIO',3,0,30),
('P0003','Cuisses de poulet','LabelRouge',6,0,25),
('P0004','Oeuf','BIO',15,6,50),
('P0005','Carottes','BIO',3,0,20),
('P0006','Riz sec','BIO',7,2,40),
('P0007','Farine','BIO',14,10,80),
('P0008','Beurre','BIO',12,0,30),
('P0009','Pate Brisée','BIO',12,0,30),
('P0010','Levure', 'BIO', 10, 2, 20),
('P0011', 'Sel', 'sel fin', 25, 0, 30);

-- filling recette
insert into recette (`idRecette`,`nom_R`,`type`,`descriptif`,`prix_vente`,`nb_commande`,`temps_conservation`,`idClient`) VALUES
('R0001','Cake Banane','Dessert','Cake leger',3,5,10,'A0001'),
('R0002','Tarte Pomme','Dessert','Tarte',4,2,4,'A0005'),
('R0003','Omelette','Salé','Plat',18,8,1,'A0007'),
('R0004','Poulet/Riz','Salé','Plat',9,2,4,'A0009'),
('R0005','Carottes râpées','Salé','Plat',23,3,4,'A0009');

-- filling listeingredients
insert into listeingredients (`idProduit`,`idRecette`,`unite_quantite`) VALUES
('P0001','R0001',4),
('P0002','R0002',10),
('P0003','R0004',4),
('P0004','R0003',12),
('P0004','R0001',12),
('P0005','R0005',15),
('P0006','R0004',13),
('P0007','R0001',10),
('P0008','R0001',2),
('P0009','R0002',5);

-- filling commande
insert into commande (`idCommande`,`prixCommande`,`date_commande`,`idClient`) VALUES
('COM0001',20,'2019-7-4','A0001'),
('COM0002',20,'2019-8-2','A0005'),
('COM0003',10,'2020-8-14','A0009'),
('COM0004',15,'2019-12-17','A0009'),
('COM0005',12,'2019-10-13','A0007');


-- filling contient
insert into contient (`idCommande`,`idRecette`,`quantite_commande`) VALUES
('COM0001','R0001',2),
('COM0002','R0002',4),
('COM0003','R0004',1),
('COM0004','R0003',2),
('COM0005','R0005',3);

-- filling fournisseur
insert into fournisseur (`idFournisseur`,`nom_F`,`num_tel_F`) VALUES
('F0001','Carrefour','0678450932'),
('F0002','Auchan','0647830921'),
('F0003','Leclerc','0657939572'),
('F0004','MarchéBio','0799424503'),
('F0005','HyperU','0647834389'),
('F0006','LeaderPrice','0666738291');

-- filling fournit_stock
insert into fournit_stock (`idFournisseur`,`idProduit`,`ref_fournisseur`,`date_ajout`) VALUES
('F0001','P0001','BN1','2019-9-8'),
('F0002','P0002','P1','2019-10-3'),
('F0003','P0004','V2','2020-1-27'), 
('F0004','P0003','V0','2019-7-5'), -- V0 pour viande blanche, V1 pour viande rouge etc (fixé arbitrairement)
('F0005','P0005','C1','2020-2-18'),
('F0006','P0006','R1','2020-1-16'),
('F0006','P0007','F1','2020-1-16'),
('F0006','P0008','B1','2020-1-16'),
('F0006','P0009','PB1','2020-1-16');

-- filling prepare
insert into prepare (`idCuisinier`,`idRecette`,`date_preparation`) VALUES
('C0001','R0001','2019-8-4'),
('C0002','R0002','2019-8-19'),
('C0004','R0004','2020-3-3'),
('C0005','R0005','2019-9-12'),
('C0009','R0003','2019-12-17');
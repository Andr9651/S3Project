Create Database CookieClicker 

go

use CookieClicker

Create table GameData (
	id int primary key identity not null,
	balance int,
	hostIp varchar(32)
);

create table Purchasable(
	id int primary key identity not null, 
	name varchar(128),
	price int
); 

create table GamePurchase(
	gameDataId int foreign key references GameData(id) not null,
	purchasableId int foreign key references Purchasable(id) not null,
	primary key (gameDataId, purchasableId),
	amount int not null,
);

use master
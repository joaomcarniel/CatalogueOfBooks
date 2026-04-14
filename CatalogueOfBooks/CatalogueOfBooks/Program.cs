using CatalogueOfBooks.Services;
using CatalogueOfBooks.UI;

var storage = new FileStorageService();
var catalogue = new CatalogueService(storage);
var menu = new MainMenu(catalogue);

menu.RunMenu();
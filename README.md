Hej Hans (lär bara vara du som läser detta)! 

Tuff uppgift, och jag har inte riktigt kommit i hamn med allt jag skulle vilja fixa, men tiden är knapp! 

För att börja använda appen har jag skapat en seeding-klass som skapar några grund-entiteter för att appen ska kunna börja användas direkt. 
Användarnamnet admin@domain.com med lösenordet "Admin@123456" släpper in dig som admin så du kan komma åt alla de funktionerna som admin har tillgång till. 

Jag tänkte också att jag kan ge dig en lista med problem jag känner till och hade velat fixa om jag hade haft tid: 
* Notifikationer kan dismissas, men de kommer tillbaka när man laddar om sidan.
* Vissa darkmode-detaljer fungerar inte som de ska.
* Darkmode-val sparas även till nästa användare - och jag har inte lyckats få javascriptet att ha i åtanke om användaren har valt darkmode som standard i sin webbläsare.
* Mina cookies fungerar enligt din tutorial, men jag har inte hunnit konfigurera de faktiska cookiesarna. Där har jag också samma problem som med darkmode - valen följer inte användaren. 
* Jag har förberett för chat messages, men inte hunnit implementera funktionen.
* Jag har inte hunnit lägga till något innehåll till Settings-menyn.
* I nuläget syns inga felmeddelanden (förutom valideringsfel) utåt till användaren.
* Mina kommentarer om AI-genererad kod är inte klockrena. I många fall har jag behövt gå tillbaka om och om igen och göra justeringar, och då jag har använt både Claude AI (mest) och ChatGPT 4o är jag på vissa ställen osäker på vilken av dem som hjälpt med vad.
* En del designbitar är inte vackra och det är lite inkonsekvens här och var med bildsökvägar och annat som gör att användarens valda bild inte alltid dyker upp där den ska.
* Admin-login-sidan finns och nås via inloggningsformuläret, men i dagsläget finns det ingenting som gör att den behövs - och det finns inga restriktioner som gör att inte vanliga användare kommer in den vägen (dock till sin restrictade access). 

Med allt detta sagt, jag hoppas det mesta andra funkar som det ska!

Mvh, 

Jasmin

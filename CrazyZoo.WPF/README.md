Crazy Zoo

Projekti kirjeldus
Crazy Zoo on C# ja WPF abil loodud rakendus, kus kasutaja saab erinevate loomadega tegevusi teha.
Rakenduses on neli looma:

*Monkey – ahv
*Penguin – pingviin
*Lion – lõvi
*Parrot – papagoi

Igal looma on nimi, energia, õnnelikkus ja oma CrazyAction.
Projekti struktuur

*CrazyZoo.Core – klassid, liidesed ja programmi loogika.
*CrazyZoo.WPF – graafiline kasutajaliides.

Klassid ja liidesed
Animal on abstraktne baasklass.
Alamklassid: Monkey, Penguin, Lion, Parrot.

Liidesed:

*ISwimmable – ujumine
*IClimb – ronimine
*Ifly – lendamine
*ITalk – rääkimine

CrazyAction

*Monkey korraldab banaanipeo.
*Penguin alustab jäätantsu.
*Lion teeb väga valju möirge.
*Parrot jäljendab loomaaia töötajat.
*CrazyAction muudab looma energiat ja õnnelikkust.

WPF kasutajaliides
Rakenduses kasutatakse ObservableCollection, DataTemplate, ProgressBar, Expander ja TabControl elemente.
ProgressBar võimaldab jälgida looma energiat ja õnnelikkust.

Testimine

*Ahvi ronimisel väheneb energia 15 võrra.
*Pingviin ei saa lennata.
*Lõvi CrazyAction vähendab energiat 25 võrra.
*Papagoi lendamisel väheneb energia 20 võrra.
*Papagoi rääkimisel väheneb energia 5 võrra.
*Tühja nimega looma ei saa lisada.
*Kui energiat ei ole piisavalt, tegevust ei toimu.

Käivitamine

1.Ava CrazyZoo.sln Visual Studios.

2.Vali käivitusprojektiks CrazyZoo.WPF.

3.Vajuta F5.

GitHub koostöö

Issue: [Lisa link]

Pull Request: [Lisa link]

Code Review: [Lisa kirjeldus]

AI kasutamine

Kasutasin ChatGPT-d C# ja WPF mõistete selgitamiseks ning näidiskoodi koostamiseks. Kontrollisin koodi ja tegin vajalikud parandused.
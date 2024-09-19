# Arbeitsprotokoll

## Preprocessing

Die Idee hinter dem Preprocesisng war es die Daten aus der CSV Datei in eine einfach zu handhabende Datenstruktur zu überführen.

Grundsätzlich war die Überlegung eine Art Heap für die Daten zu erstellen, in dem die Daten in einer Baumstruktur abgelegt werden. Die Baumstruktur sollte dabei so aufgebaut sein, dass die Daten in der hierarchischen Struktur der Repräsentation der Stadt entsprechen. D.h. zuerst haben wir uns überlegt, wie die Stadt und die Steuerung grundsätzlich aufgebaut sind und wie wir diese Struktur in einer Baumstruktur abbilden können. So haben wir die Daten in folgender Reihenfolge strukturiert:

Jahr -> Stadt -> Gebäudeklasse -> Facilities -> Gebäude

Im Prinzip hat jedes Jahr mehrere Städte, jede Stadt mehrere Gebäudeklassen, jede Gebäudeklasse mehrere Facilities und jede Facility mehrere Gebäude.
Mit dieser Überlegung haben wir das Preprocessing implementiert, wobei wir Zeile für Zeile durch die CSV Datei iteriert haben und die Daten in die entsprechende Struktur abgelegt haben. Die Überlegung da war falls aus der ersten Hierarchieebene ein Wert fehlt, wird die Zeile als komplett neues Element angelegt. Falls es bereits ein Element aus dieser Hierarchieebene gibt, so wird der Wert der nächsten Hierarchieebene sich angeschaut und die Schritte werden wiederholt. So garantiert man, dass am Ende die Daten in der richtigen Struktur abgelegt sind.

Einige Probleme bei der Entwicklung waren:

- die Schleife musste zum Teil mehrfach durchlaufen werden, da wir Durschnittswerte für gewisse Kategorien berechnen mussten
- es gab in der Entwicklungsphase neue Erweiterungswünsche, weshalb die Struktur des Baumes mehrmals angepasst werden musste
- wir haben recht simpel versucht die Liste zu sortieren, aber aus einem uns unerklärlichen Grund hat dann die Stadtgenerierung nicht funktioniert

## Kamerasteuerung



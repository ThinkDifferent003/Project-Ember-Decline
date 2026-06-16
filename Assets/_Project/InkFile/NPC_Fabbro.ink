VAR Livello_Spada = 1
VAR Minerali_Giocatore = 1
VAR Parlato_Con_Fabbro = false
FORWARD Cost_Upgrade(lvl)
=== NPC_Fabbro  ===

{ Parlato_Con_Fabbro == false:
 -> Prima_Volta
 - else:
  -> Menu_Fabbro
}
= Prima_Volta
~ Parlato_Con_Fabbro = true
"Sei qui per farti rimettere in sesto la spada, o hai volgia di sprecare il mio tempo?"
-> Menu_Fabbro
= Menu_Fabbro
"Cosa ti serve?"
~ temp costo = Cost_Upgrade(Livello_Spada)
 +[Potenzia Arma]
   { Minerali_Giocatore >= costo:
     -> esegui_potenziamento(costo)
   - else:
     "Sei senza risorse. Non lavoro gratis. Ti servono {costo} minerali. Sparisci"
     -> Menu_Fabbro
   }
  +[Chiedi del suo braccio]
    "Questo? Una mia vecchia invenzione..."
    -> Menu_Fabbro
  +[Arrivederci]
    "Addio"
    -> END
= esegui_potenziamento(costo)
~ Minerali_Giocatore = Minerali_Giocatore - costo
~ Livello_Spada = Livello_Spada + 1
"Ecco fatto. Ora la tua lama è piu affilata"
# EVENTO: UpgradeSpada_{Livello_Spada}
# EVENTO: SincronizzaMinerali_{Minerali_Giocatore}
-> END

== function Cost_Upgrade(lvl) ==
{ lvl:
 - 1: ~ return 1
 - 2: ~ return 3
 - 3: ~ return 6
 - else ~ return 10
} 

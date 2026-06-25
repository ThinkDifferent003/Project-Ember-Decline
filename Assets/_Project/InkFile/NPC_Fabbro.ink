VAR Livello_Spada = 0
VAR Minerali_Giocatore = 1
VAR Parlato_Con_Fabbro = false
VAR Potenziamento_Max_Discusso = false
FORWARD Cost_Upgrade(lvl)
=== NPC_Fabbro  ===
{ Livello_Spada == 3:
  -> Menu_Fabbro_Amichevole
}  
{ Parlato_Con_Fabbro == false:
 -> Prima_Volta
 - else:
  -> Menu_Fabbro
}
= Prima_Volta
~ Parlato_Con_Fabbro = true
"Quell'accento... vieni da *lì*, vero? Sento l'odore delle ceneri di quel regno fin da qui. Sei qui per farti rimettere in sesto la spada, o hai volgia di sprecare il mio tempo?"
-> Menu_Fabbro
= Menu_Fabbro
{ Livello_Spada:
  - 0:"Cosa ti serve? Sbrigati."
  - 1:"Ancora tu. Vuoi qualcos'altro?"
  - 2:"Sei sopravvissuto allora. Ti porti addosso i tagli di chi scappa da qualcosa, non di chi serve una corona. Dimmi, serve una sistemata alla lama?"
}  
~ temp costo = Cost_Upgrade(Livello_Spada)
 +[Potenzia Arma]
   { Minerali_Giocatore >= costo:
     -> esegui_potenziamento(costo)
   - else:
     "Sei senza risorse. Non lavoro gratis. Ti servono {costo} minerali. Sparisci"
     -> Menu_Fabbro
   }
  +{Livello_Spada >= 1} [Chiedi del suo braccio]
    { Livello_Spada == 1:
      "Questo? Una mia vecchia invenzione...un promemoria di quello che ero..."
    }
    {Livello_Spada == 2:
      "Ancora curioso? L'ho creato molti anni fa, ero un inventore... ma ho anche creato cose orribili.."
    }  
    -> Menu_Fabbro
  +[Arrivederci]
    "Addio"
    -> END
= esegui_potenziamento(costo)
~ Minerali_Giocatore = Minerali_Giocatore - costo
~ Livello_Spada = Livello_Spada + 1
# EVENTO: UpgradeSpada_{Livello_Spada}
# EVENTO: SincronizzaMinerali_{Minerali_Giocatore}
{Livello_Spada:
   - 1: "Ecco fatto. Ora la tua lama è piu affilata."
    -> END
   - 2: "Fatto. Sento che capisci a capire il peso di una vera arma. Fa paura, vero? Sapere che un pezzo di metallo freddo può spezzare una vita in un battito di ciglia..."
    -> END
   - 3: "Ci siamo. Il massimo che posso tirare fuori da questo pezzo di ferro. È micidiale. Ho... ho riversato troppa precisione in questa lama. Mi ricorda... No, lasciami solo. Sparisci, devo riflettere."
    -> END
}   

{Livello_Spada == 4:
  -> Menu_Fabbro_Amichevole
 - else:
  -> END
}  
= Menu_Fabbro_Amichevole
{ Potenziamento_Max_Discusso:
  "Bentronato"
 - else:
  ~ Potenziamento_Max_Discusso = true
  "Ah! Guarda chi si rivede! *Eheh*, il ragazzo con la lama più letale della zona. Sai... all'inizio ti odiavo solo per la terra da cui provieni. Ma tu non sei come loro. Sei un sopravvissuto, proprio come me. Cosa posso fare per te? Cosa posso fare per te?"
}  
 +[Parla del futuro]
  "Ora che la tua arma è perfetta, non ti resta che finire il lavoro là fuori."
  -> Menu_Fabbro_Amichevole
 +[Chiedi del suo passato]
  "Sono colpevole. Con le mie creazioni, molte persone sono state uccise... è colpa mia. Il sangue di chi amavo... è sulle mie mani."
  -> Menu_Fabbro_Amichevole
 +[Arrivederci]
  "Fai attenzione là fuori"
  -> END

== function Cost_Upgrade(lvl) ==
{ lvl:
 - 0: ~ return 1
 - 1: ~ return 3
 - 2: ~ return 6
 - else ~ return 999
} 

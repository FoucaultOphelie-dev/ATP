﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public CharacterMove characterMove;
    public ParkourManager parkourManager;
    public TimeManager timeManager;

    private bool tutorialStarted = false;
    private bool taughtToWalk = false;
    private bool taughtToJump = false;
    private bool taughtToRun = false;
    private bool taughtToSlide = false;
    private bool taughtToWallride = false;
    private bool taughtToJumpWhileWallriding = false;
    private bool taughtToGrab = false;
    private bool taughtMovingPlatforms = false;
    private bool taughtFragilePlatforms = false;
    private bool taughtAimingAndShooting = false;
    private bool taughtReloading = false;
    private bool taughtDestructibleObstacle = false;
    private bool taughtPower = false;
    private bool taughtGameBasics = false;
    private string keyTextColor;

    public CheckPoint[] checkpoints;
    public PopupSystem popUp;

    public string textColorHexCode = "FFFFFF";
    private string tutorialText;

    //public TimeManager timeManager;

    // Start is called before the first frame update
    void Start()
    {
        keyTextColor = "<#"+textColorHexCode+">";
    }

    // Update is called once per frame
    void Update()
    {
        if(!tutorialStarted && !taughtGameBasics)
        {
            teachGameBasics();
        }

        if (tutorialStarted && !taughtToWalk)
        {
            teachToWalk();
        }

        if (checkpoints[0].isChecked() && !taughtToJump)
        {
            teachToJump();
        }

        if(checkpoints[1].isChecked() && !taughtToRun)
        {
            teachToRun();
        }

        if(checkpoints[2].isChecked() && !taughtToSlide)
        {
            teachToSlide();
        }

        if(checkpoints[3].isChecked() && !taughtToWallride)
        {
            teachToWallride();
        }

        if(checkpoints[4].isChecked() && !taughtToJumpWhileWallriding)
        {
            teachToJumpWhileWallriding();
        }

        if(checkpoints[5].isChecked() && !taughtToGrab)
        {
            teachToGrab();
        }

        if(checkpoints[6].isChecked() && !taughtMovingPlatforms)
        {
            teachMovingPlatforms();
        }

        if(checkpoints[7].isChecked() && !taughtFragilePlatforms)
        {
            teachFragilePlatforms();
        }

        if(checkpoints[8].isChecked() && !taughtAimingAndShooting)
        {
            teachAimingAndShooting();
        }

        if(checkpoints[9].isChecked() && !taughtReloading)
        {
            teachReloading();
        }

        if(checkpoints[10].isChecked() && !taughtDestructibleObstacle)
        {
            teachDestructibleObstacle();
        }

        if(checkpoints[11].isChecked() && !taughtPower)
        {
            teachPower();
        }
    }

    private void teachGameBasics()
    {
        //timeManager.pauseGame();
        tutorialText = "Bienvenue dans ton premier parcours. Avant de démarrer un parcours, une caméra de présentation te montreras le chemin à suivre. Cela permet de se faire une première idée du parcours.\n\n\n Pour te lancer dans le pourcours, appuies sur la touche " + keyTextColor + KeyCode.Return.ToString()+"</color>";
        popUp.openPopUp(tutorialText);
        taughtGameBasics = true;
    }

    private void teachToWalk()
    {
        //timeManager.pauseGame();
        tutorialText = "Dans chaque parcours, tu dois franchir tous les points de passage le plus rapidement possible ! Pour cela, tu vas devoir te déplacer : \n\n\n - Pour se déplacer vers la gauche utilises la touche " + keyTextColor + KeyCode.Q.ToString() + "</color>" + "\n - Pour se déplacer vers l'avant, utilises la touche " + keyTextColor + KeyCode.Z.ToString() + "</color>" + "\n - Pour se déplacer vers la droite, utilises la touche " + keyTextColor + KeyCode.D.ToString() + "</color>" + "\n - Pour se déplacer vers l'arrière, utilises la touche " + keyTextColor + KeyCode.S.ToString() + "</color>" + "\n - Pour revenir au dernier point de passage franchi, utilises la touche " + keyTextColor + parkourManager.softResetKey + "</color>" + "\n - Pour revenir au début du parcours, utilises la touche " + keyTextColor + parkourManager.hardResetKey + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToWalk = true;
    }

    private void teachToJump()
    {
        //timeManager.pauseGame();
        tutorialText = "Tu peux sauter au dessus des obstacles pour les franchir.\n\n\n Pour sauter, appuies sur " + keyTextColor + characterMove.keyJump.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToJump = true;
    }

    private void teachToRun()
    {
        //timeManager.pauseGame();
        tutorialText = "Courir permet de gagner de la vitesse et de sauter par dessus des obstacles plus longs.\n\n\n Pour courir appuies sur " + keyTextColor + characterMove.keyRun.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToRun = true;
    }

    private void teachToSlide()
    {
        //timeManager.pauseGame();
        tutorialText = "Il sera parfois nécessaire de glisser sous certains obstacles pour les franchir.\n\n\n Pour glisser, appuyer sur la touche " + keyTextColor + characterMove.keySlide.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToSlide = true;
    }

    private void teachToWallride()
    {
        //timeManager.pauseGame();
        tutorialText = "Les murs verticaux sont parfaits pour gagner un peu de distance avant un saut. Il est possible d'utiliser ces murs pour prendre quelques pas d'appuis.\n\n\n Pour courir sur un mur, courir vers le mur et appuyer sur " + keyTextColor + characterMove.keyJump.ToString() + "</color>" + " pour s'élancer dessus.";
        popUp.openPopUp(tutorialText);
        taughtToWallride = true;
    }

    private void teachToJumpWhileWallriding()
    {
        //timeManager.pauseGame();
        tutorialText = "Tout en courant sur les murs, il est possible de sauter pour se donner un peu de distance supplémentaire ou pour passer d'un mur à un autre.\n\n\n Pour sauter depuis un mur, appuis sur la touche " + keyTextColor + characterMove.keyJump.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToJumpWhileWallriding = true;
    }

    private void teachToGrab()
    {
        //timeManager.pauseGame();
        tutorialText = "Lors d'un saut où tu manques légèrement de hauteur ou de distance, tu peux t'aggriper aux rebords d'une plateforme. Une fois aggripé, tu peux soit monter sur la plateforme, soit sauter depuis le rebord de la plateforme.\n\n\n Pour monter sur la plateforme, utilises la touche <#FFFFFF>\"avancer\"</color>.\n\n Pour sauter depuis le rebords de la plateforme, utilises la touche " + keyTextColor + characterMove.keyJump + "</color>";
        popUp.openPopUp(tutorialText);
        taughtToGrab = true;
    }

    private void teachMovingPlatforms()
    {
        //timeManager.pauseGame();
        tutorialText = "Certaines platformes se déplacent, il faudra que tu sautes dessus au bon moment.";
        popUp.openPopUp(tutorialText);
        taughtMovingPlatforms = true;
    }

    private void teachFragilePlatforms()
    {
        //timeManager.pauseGame();
        tutorialText = "Certaines plateformes sont fragiles et s'écrouleront sous tes pas. Traverses les le plus vite possible pour ne pas tomber !";
        popUp.openPopUp(tutorialText);
        taughtFragilePlatforms = true;
    }

    private void teachAimingAndShooting()
    {
        //timeManager.pauseGame();
        tutorialText = "Tout au long du parcours, tu croiseras des cibles. Pour gagner encore plus de points il faudra tirer sur ces cibles. Plus tu tireras au centre d'une cible, plus tu gagneras de points !\nAttention, toutes les cibles ne valent pas le même nombre de points ! Les cibles bleues valent 100 points. Les cibles rouges valent 200 points. Attention tirer sur une cible verte te feras perdre des points !\n\n - Pour dégainer ton pistolet, utilise ton <#FFFFFF>clique droit</color>\n - Pour tirer, utilise le <#FFFFFF>clique gauche" + "</color>";
        popUp.openPopUp(tutorialText);
        taughtAimingAndShooting = true;
    }

    private void teachReloading()
    {
        //timeManager.pauseGame();
        tutorialText = "Lorsque tu n'as plus de munitions, tu devras recharger ! Attention, pour recharger, tu dois avoir ton arme en main !\n\n\n Pour recharger, utilise la touche " + keyTextColor + KeyCode.R.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtReloading = true;
    }

    private void teachDestructibleObstacle()
    {
        //timeManager.pauseGame();
        tutorialText = "Certains obstacles sont imposants mais sont aussi fragile. Cela signifie qu'ils peuvent être détruit.\n\n\n Pour détruire un obstacle, utilises ton arme pour tirer dessus";
        popUp.openPopUp(tutorialText);
        taughtDestructibleObstacle = true;
    }

    private void teachPower()
    {
        //timeManager.pauseGame();
        tutorialText = "Certaines platformes ou cibles se déplacent trop vite pour être facilement apréhendables. Pour pouvoir faire face à ces situations tu possède le pouvoir de maitriser le temps. Lorsque tu accélère le temps, ta jauge de temps augmente. Tu peux alors consommer cette jauge de temps pour le ralentir. Ralentir le temps te permettra de mieux assurer tes tirs et tes sauts sur des objets très rapides\n\n\n - Pour accélerer le temps, utilises la touche " + keyTextColor + timeManager.keyAcceleration.ToString() + "</color>" + "\n - Pour ralentir le temps, utilise la touche " + keyTextColor + timeManager.keyRalenti.ToString() + "</color>";
        popUp.openPopUp(tutorialText);
        taughtPower = true;
    }

    public void startTutorial()
    {
        tutorialStarted = true;
    }
}

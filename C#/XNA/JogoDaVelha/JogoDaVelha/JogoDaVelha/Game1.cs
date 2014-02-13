using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JogoDaVelha
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D planoDeFundo;         // textura de plano de fundo
        Texture2D sprite;               // textura do X e O
        Texture2D ponteiro;             // textura do ponteiro do mouse

        List<Rectangle> retangulos;     // variavel que guarda os retangulo onde você pode colocar X e O

        Vector2 posicao;                //

        int index;
        int[] tabuleiro = new int[9];
        int jogador;
        
        bool terminouJogo;
        
        SpriteFont fonte;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            graphics.PreferredBackBufferWidth = 360;
            graphics.PreferredBackBufferHeight = 440;
            graphics.IsFullScreen = false;

            Window.Title = "Jogo da Velha - XNA";
            graphics.ApplyChanges();
        }

        /// <summar
        protected override void Initialize()
        {

            posicao = new Vector2();

            geraRetangulosClicaveis();

            NewGame();

            base.Initialize();

        }

        public void geraRetangulosClicaveis()
        {
            // Retangulos do tabuleiro
            retangulos = new List<Rectangle>();
            // Primeira linha
            retangulos.Add(new Rectangle(6, 46, 108, 108));
            retangulos.Add(new Rectangle(126, 46, 108, 108));
            retangulos.Add(new Rectangle(246, 46, 108, 108));
            // Segunda linha
            retangulos.Add(new Rectangle(6, 166, 108, 108));
            retangulos.Add(new Rectangle(126, 166, 108, 108));
            retangulos.Add(new Rectangle(246, 166, 108, 108));
            // Terceira linha
            retangulos.Add(new Rectangle(6, 286, 108, 108));
            retangulos.Add(new Rectangle(126, 286, 108, 108));
            retangulos.Add(new Rectangle(246, 286, 108, 108));
        }

        public void NewGame()
        {
            // Inicializa o vetor do tabuleiro com 0
            for (int i = 0; i < 9; i++)
                tabuleiro[i] = 0;

            jogador = 1;

            terminouJogo = false;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            planoDeFundo = Content.Load<Texture2D>(@"Texturas\planodefundo");
            sprite = Content.Load<Texture2D>(@"Texturas\sprite");
            ponteiro = Content.Load<Texture2D>(@"Texturas\ponteiro");
            fonte = Content.Load<SpriteFont>(@"Fontes\arial");
        }


        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Recebe o estado atual do teclado

            estadoDasTeclas();

            verificaPosicoesMarcadas();

            base.Update(gameTime);
        }

        public void estadoDasTeclas()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.N))
                NewGame();
            else if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

        }

        public void verificaPosicoesMarcadas()
        {
            for (int i = 0; i < 2; i++)
            {
                if ((tabuleiro[0] == i + 1 && tabuleiro[1] == i + 1 && tabuleiro[2] == i + 1) ||
                    (tabuleiro[3] == i + 1 && tabuleiro[4] == i + 1 && tabuleiro[5] == i + 1) ||
                    (tabuleiro[6] == i + 1 && tabuleiro[7] == i + 1 && tabuleiro[8] == i + 1) ||
                    (tabuleiro[0] == i + 1 && tabuleiro[3] == i + 1 && tabuleiro[6] == i + 1) ||
                    (tabuleiro[1] == i + 1 && tabuleiro[4] == i + 1 && tabuleiro[7] == i + 1) ||
                    (tabuleiro[2] == i + 1 && tabuleiro[5] == i + 1 && tabuleiro[8] == i + 1) ||
                    (tabuleiro[0] == i + 1 && tabuleiro[4] == i + 1 && tabuleiro[8] == i + 1) ||
                    (tabuleiro[2] == i + 1 && tabuleiro[4] == i + 1 && tabuleiro[6] == i + 1))
                {
                    terminouJogo = true;
                    jogador = i + 1;
                }
            }

            if (!terminouJogo)
            {
                // Verifica se ainda existe espaço em branco no tabuleiro 
                bool empty = false;
                for (int i = 0; i < 9; i++)
                    if (tabuleiro[i] == 0)
                        empty = true;

                // Se acabou os espaços vazios finaliza o jogo empatado
                if (!empty)
                {
                    terminouJogo = true;
                    jogador = 0;
                }
            }

            MouseState mouse = Mouse.GetState();
            posicao.X = mouse.X;
            posicao.Y = mouse.Y;

            if (!terminouJogo)
            {
                // Verifica se a posição do mouse esta dentro de algum espaço que possa prrencher com X ou O
                index = -1;
                int count = 0;
                foreach (Rectangle rect in retangulos)
                {
                    if (rect.Contains(mouse.X, mouse.Y))
                        index = count;

                    count++;
                }

                // Verifica se o botão esquerdo do mouse foi pressionado
                if (mouse.LeftButton == ButtonState.Pressed && index != -1)
                {
                    if (tabuleiro[index] == 0)
                    {
                        tabuleiro[index] = jogador;
                        jogador = jogador == 1 ? 2 : 1;
                    }
                }
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin();

                desenhaPlanoDeFundo();

                desenhaPosicoesOcupadas();

                verificaTerminoJogo();

                desenhaPonteiro();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void desenhaPlanoDeFundo()
        {
            spriteBatch.Draw(planoDeFundo, Vector2.Zero, Color.White);
        }

        public void desenhaPosicoesOcupadas()
        {
            for (int i = 0; i < 9; i++)
            {
                if (tabuleiro[i] == 1)
                    spriteBatch.Draw(sprite, retangulos[i], new Rectangle(0, 0, 108, 108), Color.White);
                if (tabuleiro[i] == 2)
                    spriteBatch.Draw(sprite, retangulos[i], new Rectangle(0, 108, 108, 108), Color.White);
            }

            if (!terminouJogo)
            {
                // Figura da próxima peça numa determinada posiçao
                if (index != -1 && tabuleiro[index] == 0)
                {
                    if (jogador == 1)
                        spriteBatch.Draw(sprite, retangulos[index], new Rectangle(108, 0, 108, 108), Color.White);
                    else
                        spriteBatch.Draw(sprite, retangulos[index], new Rectangle(108, 108, 108, 108), Color.White);
                }
            }
        }

        public void verificaTerminoJogo()
        {
            if (terminouJogo)
            {

                spriteBatch.Draw(planoDeFundo,
                    Vector2.Zero,
                    Color.Black);

                String str;

                if (jogador == 0)
                    str = "Jogaram muito bem, mas empataram.\nPressione N para jogar novamente!";
                else
                    str = "Jogador " + jogador.ToString() + " venceu!\nPressione N para jogar novamente.";

                Vector2 textSize = fonte.MeasureString(str);
                spriteBatch.DrawString(fonte,
                    str,
                    new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / 2 - textSize / 2,
                    Color.White);
            }
        }

        public void desenhaPonteiro()
        {
            spriteBatch.Draw(ponteiro, posicao, Color.White);
        }


    }
}



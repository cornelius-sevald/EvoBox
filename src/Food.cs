using System;
using evobox.Graphical;

namespace evobox
{

    /// <summary>
    /// An entity for jumpmen to eat.
    /// </summary>
    public class Food : Entity
    {

        const string FOOD_NAME = "sprites/food_tomato.png";
        const string FOOD_SMALL_NAME = "sprites/food_ketchup_small.png";
        const string FOOD_BIG_NAME = "sprites/food_ketchup_large.png";

        private double nutrition = 0;

        /// <summary>
        /// Create food with a certain nutritional value.
        /// </summary>
        public Food(Vector2 position, Vector2 scale, int zIndex, double nutrition)
        : base(position, scale, zIndex)
        {
            this.nutrition = nutrition;

            if (nutrition < 5)
            {
                this.texture = new Texture(Globals.RENDERER, FOOD_NAME);
            }
            else if (nutrition < 15)
            {
                this.texture = new Texture(Globals.RENDERER, FOOD_SMALL_NAME);
            }
            else
            {
                this.texture = new Texture(Globals.RENDERER, FOOD_BIG_NAME);
            }
        }
    }
}

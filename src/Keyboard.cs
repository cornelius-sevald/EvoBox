using System;

using SDL2;

namespace evobox {

    /// <summary>
    /// Wrapper of a pointer to a Uint8 array.
    /// This class is a singleton.
    /// </summary>
    public sealed class Keyboard {

        /// <summary>
        /// Pointer to the internal UInt8 (byte) array.
        /// </summary>
        public IntPtr ArrPointer { get; private set; }
        public int numKeys;

        /// <summary>
        /// Get the single instance of the keyboard state.
        /// </summary>
        public static Keyboard Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Keyboard();
                    }
                    return instance;
                }
            }
        }

        private static readonly object padlock = new object();
        private static Keyboard instance = null;

        Keyboard () {
            int numKeys;
            this.ArrPointer = SDL.SDL_GetKeyboardState(out numKeys);
            this.numKeys = numKeys;
        }

        public byte this[SDL.SDL_Scancode sc] {
            get {
                int i = (int) sc;
                unsafe {
                    byte *arr = (byte *) ArrPointer.ToPointer();
                    byte key = *(arr + i);
                    return key;
                }
            }
        }
    }

}


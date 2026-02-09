import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

export const NexusTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50: '{blue.50}',
      100: '{blue.100}',
      200: '{blue.200}',
      300: '{blue.300}',
      400: '{blue.400}',
      500: '{blue.500}',
      600: '{blue.600}',
      700: '{blue.700}',
      800: '{blue.800}',
      900: '{blue.900}',
      950: '{blue.950}',
    },
    focusRing: {
      width: '0',
      style: 'none',
      color: 'transparent',
      offset: '0',
      shadow: 'none',
    },
    colorScheme: {
      light: {
        primary: {
          color: '{blue.950}',
          contrastColor: '{surface.0}',
          hoverColor: '{blue.900}',
          activeColor: '{blue.800}',
        },
        highlight: {
          background: '{blue.100}',
          focusBackground: '{blue.200}',
          color: '{blue.950}',
          focusColor: '{blue.950}',
        },
        formField: {
          focusBorderColor: '{blue.950}',
        },
      },
      dark: {
        primary: {
          color: '{surface.0}',
          contrastColor: '{blue.950}',
          hoverColor: '{surface.100}',
          activeColor: '{surface.200}',
        },
        highlight: {
          background: 'color-mix(in srgb, {surface.0}, transparent 88%)',
          focusBackground: 'color-mix(in srgb, {surface.0}, transparent 80%)',
          color: '{surface.0}',
          focusColor: '{surface.0}',
        },
        formField: {
          focusBorderColor: '{surface.0}',
        },
      },
    },
  },
  components: {
    button: {
      colorScheme: {
        light: {
          root: {
            primary: {
              background: '{blue.950}',
              hoverBackground: '{blue.900}',
              activeBackground: '{blue.800}',
              borderColor: '{blue.950}',
              hoverBorderColor: '{blue.900}',
              activeBorderColor: '{blue.800}',
              color: '{surface.0}',
              hoverColor: '{surface.0}',
              activeColor: '{surface.0}',
              focusRing: {
                color: '{blue.950}',
                shadow: 'none',
              },
            },
          },
        },
        dark: {
          root: {
            primary: {
              background: '{surface.0}',
              hoverBackground: '{surface.100}',
              activeBackground: '{surface.200}',
              borderColor: '{surface.0}',
              hoverBorderColor: '{surface.100}',
              activeBorderColor: '{surface.200}',
              color: '{blue.950}',
              hoverColor: '{blue.950}',
              activeColor: '{blue.950}',
              focusRing: {
                color: '{surface.0}',
                shadow: 'none',
              },
            },
          },
        },
      },
    },
    toggleswitch: {
      colorScheme: {
        light: {
          root: {
            checkedBackground: '{blue.950}',
            checkedHoverBackground: '{blue.900}',
          },
          handle: {
            checkedColor: '{blue.950}',
            checkedHoverColor: '{blue.900}',
          },
        },
        dark: {
          root: {
            checkedBackground: '{surface.0}',
            checkedHoverBackground: '{surface.100}',
          },
          handle: {
            checkedColor: '{surface.0}',
            checkedHoverColor: '{surface.100}',
          },
        },
      },
    },
  },
});

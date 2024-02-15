const defaultTheme = require('tailwindcss/defaultTheme')

module.exports = {
  content: ["**/*.razor", "**/*.cshtml", "**/*.html"],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
    },
  },
  plugins: [],
}
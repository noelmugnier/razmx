const defaultTheme = require('tailwindcss/defaultTheme')

module.exports = {
  safelist: ["is-invalid", "is-valid"],
  content: ["**/*.razor", "**/*.cshtml", "**/*.html", "**/*.cs"],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
    },
  },
  plugins: [],
}
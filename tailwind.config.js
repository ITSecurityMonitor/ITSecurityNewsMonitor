module.exports = {
  theme: {
    extend: {
      zIndex: {
        'negative': -1,
      },
      colors: {
        primary: {
          100: "#fef6eb",
          200: "#f7c686",
          300: "#f4b35d",
          400: "#f2aa49",
          500: "#f1a035",
          600: "#d99030",
          700: "#c1802a",
          800: "#a97025",
          900: "#916020",
        },
        violet: {
          default: "rgba(155, 97, 255, 0.5)",
          muted: "rgba(155, 97, 255, 0.2)"
        }
      },
    },
  },
  purge: {
    content: ["./app/**/*.html.erb"],
  }
};
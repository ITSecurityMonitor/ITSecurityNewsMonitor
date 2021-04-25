module.exports = {
    separator: "_",
    theme: {
        extend: {
            boxShadow: {
                "2xl": "0px 10px -14px 14px rgb(255, 255, 255);",
            },
            margin: {
                "-1/3": "-0.08333rem;",
            },
            zIndex: {
                negative: -1,
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
                    muted: "rgba(155, 97, 255, 0.2)",
                },
            },
        },
        scale: {
            0: "0",
            25: ".25",
            50: ".5",
            75: ".75",
            90: ".9",
            95: ".95",
            97: ".97",
            100: "1",
            105: "1.05",
            110: "1.1",
            125: "1.25",
            150: "1.5",
            200: "2",
        },
    },
    variants: {
        extend: {
            translate: ["group-hover"],
            scale: ["group-hover", "hover"],
            width: ["responsive", "hover", "group-hover", "focus"],
            maxWidth: ["group-hover"],
            borderRadius: ["group-hover"],
            margin: ["group-hover"],
            visibility: ["group-hover", "hover"],
        },
        borderRadius: ["first", "last"],
        borderWidth: ["last", "first"],
        margin: ["responsive", "hover", "first"],
    },
    purge: {
        enabled: false,
        // Check if ** actually matches the files!
        content: [
            "./ITSecurityNewsMonitor/**/*.cshtml",
            "./ITSecurityNewsMonitor/**/*.scss",
        ],
    },
};

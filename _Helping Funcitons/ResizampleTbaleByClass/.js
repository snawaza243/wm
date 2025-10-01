

$(document).ready(function () {

    function makeTableColumnsResizableByClass(className) {
    // Insert CSS once if not exists
    if (!document.getElementById("resizableTableStyles")) {
        const css = `
            .${className} {
                table-layout: fixed;
                width: 100%;
            }
            .${className} th {
                width: 150px;
                position: relative;
                overflow: hidden;
            }
            .${className} .resizer {
                position: absolute;
                right: 0;
                top: 0;
                width: 5px;
                height: 100%;
                cursor: col-resize;
                user-select: none;
                z-index: 1;
            }
            .${className} th:hover .resizer {
                background-color: rgba(0, 0, 0, 0.1);
            }
        `;

        const style = document.createElement("style");
        style.id = "resizableTableStyles";
        style.innerHTML = css;
        document.head.appendChild(style);
    }

    const tables = document.querySelectorAll(`.${className}`);
    tables.forEach(table => {
        const ths = table.querySelectorAll("th");
        ths.forEach(th => {
            if (!th.style.width) {
                th.style.width = th.offsetWidth + "px";
            }

            const resizer = document.createElement("div");
            resizer.classList.add("resizer");
            th.appendChild(resizer);

            let startX, startWidth;

            resizer.addEventListener("mousedown", function (e) {
                startX = e.pageX;
                startWidth = th.offsetWidth;

                function onMouseMove(e) {
                    const newWidth = startWidth + (e.pageX - startX);
                    th.style.width = newWidth + "px";
                }

                function onMouseUp() {
                    document.removeEventListener("mousemove", onMouseMove);
                    document.removeEventListener("mouseup", onMouseUp);
                }

                document.addEventListener("mousemove", onMouseMove);
                document.addEventListener("mouseup", onMouseUp);

                e.preventDefault();
            });
        });
    });
}

    makeTableColumnsResizableByClass("resizableTable");
});

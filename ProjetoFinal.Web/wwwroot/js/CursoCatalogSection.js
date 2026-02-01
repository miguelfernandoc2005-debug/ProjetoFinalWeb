/* ============================================================
   1) BOTÕES DE MATÉRIA
   ------------------------------------------------------------
   - Atualizam o campo hidden IdMateria
   - Aplicam a classe "active" no botão selecionado
   ============================================================ */
document.addEventListener("DOMContentLoaded", function () {

    const materiaButtons = document.querySelectorAll(".materia-btn");
    const hiddenMateria = document.getElementById("IdMateria");

    materiaButtons.forEach(btn => {
        btn.addEventListener("click", () => {

            // Remove active de todos
            materiaButtons.forEach(b => b.classList.remove("active"));

            // Marca o clicado
            btn.classList.add("active");

            // Atualiza o hidden
            hiddenMateria.value = btn.dataset.id;
        });
    });


    /* ============================================================
       2) MODAL DE CURSO
       ------------------------------------------------------------
       - Preenche o modal com os dados do card clicado
       - Exibe o modal usando Bootstrap
       ============================================================ */

    const cards = document.querySelectorAll(".curso-card");
    const modalElement = document.getElementById("cursoModal");

    if (!modalElement) return;

    const modal = new bootstrap.Modal(modalElement);

    cards.forEach(card => {
        card.addEventListener("click", function () {

            // Preenche os campos do modal
            document.getElementById("modalNome").textContent = this.dataset.nome;
            document.getElementById("modalMateria").textContent = this.dataset.materia;
            document.getElementById("modalTipo").textContent = this.dataset.tipo;
            document.getElementById("modalCarga").textContent = this.dataset.carga;

            // ✅ Preenche a imagem
            const img = document.getElementById("modalImagem");
            if (img) img.src = this.dataset.imagem;

            // Exibe o modal
            modal.show();
        });
    });

});

        document.addEventListener('DOMContentLoaded', function () {
            // السلايدر
            const slides = document.querySelectorAll('.slide');
            const dots = document.querySelectorAll('.slider-dot');
            const arrows = document.querySelectorAll('.slider-arrow');
            let currentSlide = 0;
            let slideInterval = setInterval(nextSlide, 5000);

            function goToSlide(n) {
                slides[currentSlide].classList.remove('active');
                dots[currentSlide].classList.remove('active');

                currentSlide = (n + slides.length) % slides.length;

                slides[currentSlide].classList.add('active');
                dots[currentSlide].classList.add('active');
            }

            function nextSlide() {
                goToSlide(currentSlide + 1);
            }

            function prevSlide() {
                goToSlide(currentSlide - 1);
            }

            dots.forEach(dot => {
                dot.addEventListener('click', function () {
                    const slideIndex = parseInt(this.getAttribute('data-slide'));
                    goToSlide(slideIndex);
                    resetInterval();
                });
            });

            arrows[0].addEventListener('click', function () {
                prevSlide();
                resetInterval();
            });

            arrows[1].addEventListener('click', function () {
                nextSlide();
                resetInterval();
            });

            function resetInterval() {
                clearInterval(slideInterval);
                slideInterval = setInterval(nextSlide, 5000);
            }

            // زر ابدأ رحلتك
            const startButton = document.getElementById('start-journey');
            const bmiSection = document.getElementById('bmi-section');

            startButton.addEventListener('click', function () {
                bmiSection.style.display = 'block';
                bmiSection.scrollIntoView({ behavior: 'smooth' });
            });

            // حاسبة BMI
            const calculateBtn = document.getElementById('calculate');
            const resultDiv = document.getElementById('result');
            const underweightCard = document.getElementById('underweight-card');
            const normalCard = document.getElementById('normal-card');
            const overweightCard = document.getElementById('overweight-card');

            calculateBtn.addEventListener('click', function () {
                const weight = parseFloat(document.getElementById('weight').value);
                const height = parseFloat(document.getElementById('height').value);

                if (isNaN(weight) || isNaN(height) || weight <= 0 || height <= 0) {
                    resultDiv.textContent = 'يرجى إدخال قيم صحيحة للوزن والطول';
                    return;
                }

                // حساب مؤشر كتلة الجسم (BMI)
                const heightInMeter = height / 100;
                const bmi = weight / (heightInMeter * heightInMeter);

                // عرض النتيجة
                resultDiv.textContent = `مؤشر كتلة جسمك هو: ${bmi.toFixed(2)}`;

                // إخفاء جميع البطاقات أولاً
                underweightCard.classList.remove('show');
                normalCard.classList.remove('show');
                overweightCard.classList.remove('show');

                // عرض البطاقة المناسبة بناءً على النتيجة
                if (bmi < 18.5) {
                    setTimeout(function () {
                        underweightCard.classList.add('show');
                    }, 300);
                } else if (bmi >= 18.5 && bmi < 25) {
                    setTimeout(function () {
                        normalCard.classList.add('show');
                    }, 300);
                } else {
                    setTimeout(function () {
                        overweightCard.classList.add('show');
                    }, 300);
                }
            });
        });
(function () {
    const slides = document.querySelectorAll('.slide');
    let current = 0;
    let interval = setInterval(nextSlide, 5000);

    function show(index) {
        slides.forEach((s, i) => {
            s.classList.remove('active');
            if (i === index) s.classList.add('active');
        });
        current = index;
    }

    function nextSlide() {
        let next = (current + 1) % slides.length;
        show(next);
    }

    function prevSlide() {
        let prev = (current - 1 + slides.length) % slides.length;
        show(prev);
    }

    document.querySelector('.arrow-left').addEventListener('click', () => { prevSlide(); reset(); });
    document.querySelector('.arrow-right').addEventListener('click', () => { nextSlide(); reset(); });

    function reset() {
        clearInterval(interval);
        interval = setInterval(nextSlide, 5000);
    }
})();

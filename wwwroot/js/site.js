// Dak Ghar Niryat Kendra - Interactive Client Logic

document.addEventListener("DOMContentLoaded", function () {
    // 1. Tooltips & Popovers initialization
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // 2. Audio Chimes for Counter Scanning & Success
    window.playScanChime = function () {
        try {
            const audioCtx = new (window.AudioContext || window.webkitAudioContext)();
            const osc = audioCtx.createOscillator();
            const gain = audioCtx.createGain();
            osc.type = "sine";
            osc.frequency.setValueAtTime(880, audioCtx.currentTime); // A5 note
            osc.frequency.setValueAtTime(1760, audioCtx.currentTime + 0.08); // A6 note
            gain.gain.setValueAtTime(0.15, audioCtx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.01, audioCtx.currentTime + 0.2);
            osc.connect(gain);
            gain.connect(audioCtx.destination);
            osc.start();
            osc.stop(audioCtx.currentTime + 0.22);
        } catch (e) {
            console.log("Audio not supported or allowed.");
        }
    };

    // 3. Copy to clipboard helper
    window.copyToClipboard = function (text, btnElement) {
        navigator.clipboard.writeText(text).then(() => {
            const originalHtml = btnElement.innerHTML;
            btnElement.innerHTML = `<i class="bi bi-check2"></i> Copied!`;
            btnElement.classList.add("btn-success");
            btnElement.classList.remove("btn-outline-secondary");
            setTimeout(() => {
                btnElement.innerHTML = originalHtml;
                btnElement.classList.remove("btn-success");
                btnElement.classList.add("btn-outline-secondary");
            }, 2000);
        });
    };

    // 4. Barcode Simulator Quick Fill
    document.querySelectorAll(".quick-track-badge").forEach(badge => {
        badge.addEventListener("click", function () {
            const num = this.getAttribute("data-track");
            const input = document.getElementById("welcomeTrackingInput") || document.getElementById("mainTrackingInput") || document.getElementById("quickTrackingInput");
            if (input) {
                input.value = num;
                input.focus();
                if (window.playScanChime) window.playScanChime();
            }
        });
    });

    // 5. Dynamic Booking Items Manager
    const itemsTableBody = document.getElementById("bookingItemsBody");
    const addItemBtn = document.getElementById("addItemRowBtn");
    const itemsJsonInput = document.getElementById("itemsJsonInput");
    const totalDeclaredValueInput = document.getElementById("declaredValueINRInput");
    const foreignValInput = document.getElementById("declaredValueForeignInput");
    const fxRateInput = document.getElementById("exchangeRateInput");

    if (addItemBtn && itemsTableBody) {
        let itemIndex = itemsTableBody.querySelectorAll("tr").length;

        function updateTotals() {
            let totalInr = 0;
            const items = [];
            itemsTableBody.querySelectorAll("tr").forEach(row => {
                const desc = row.querySelector(".item-desc")?.value || "";
                const hs = row.querySelector(".item-hs")?.value || "";
                const qty = parseInt(row.querySelector(".item-qty")?.value || "1", 10);
                const uom = row.querySelector(".item-uom")?.value || "PCS";
                const unitPrice = parseFloat(row.querySelector(".item-unitprice")?.value || "0");
                const weight = parseFloat(row.querySelector(".item-weight")?.value || "0.5");
                const rowTotal = qty * unitPrice;

                const rowTotalEl = row.querySelector(".item-total");
                if (rowTotalEl) rowTotalEl.textContent = "₹ " + rowTotal.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

                totalInr += rowTotal;

                if (desc.trim()) {
                    items.push({
                        itemDescription: desc,
                        hsCode: hs,
                        quantity: qty,
                        unitOfMeasurement: uom,
                        unitPriceINR: unitPrice,
                        totalPriceINR: rowTotal,
                        netWeightKg: weight,
                        countryOfOrigin: "India"
                    });
                }
            });

            if (totalDeclaredValueInput) {
                totalDeclaredValueInput.value = totalInr.toFixed(2);
            }
            if (foreignValInput && fxRateInput) {
                const rate = parseFloat(fxRateInput.value || "86.50");
                if (rate > 0) {
                    foreignValInput.value = (totalInr / rate).toFixed(2);
                }
            }
            if (itemsJsonInput) {
                itemsJsonInput.value = JSON.stringify(items);
            }
        }

        addItemBtn.addEventListener("click", function () {
            itemIndex++;
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>
                    <input type="text" class="form-control form-control-sm item-desc" placeholder="e.g. Handloom Block Print Kurti" required />
                </td>
                <td>
                    <input type="text" class="form-control form-control-sm item-hs mono-font" placeholder="6204.42" required />
                </td>
                <td style="width: 90px;">
                    <input type="number" class="form-control form-control-sm item-qty" value="1" min="1" />
                </td>
                <td style="width: 100px;">
                    <select class="form-select form-select-sm item-uom">
                        <option value="PCS">PCS</option>
                        <option value="KGS">KGS</option>
                        <option value="SET">SET</option>
                        <option value="BOX">BOX</option>
                    </select>
                </td>
                <td style="width: 130px;">
                    <input type="number" step="0.01" class="form-control form-control-sm item-unitprice" value="1500" min="1" />
                </td>
                <td style="width: 100px;">
                    <input type="number" step="0.01" class="form-control form-control-sm item-weight" value="0.40" min="0.01" />
                </td>
                <td class="text-end fw-bold item-total text-dark">₹ 1,500.00</td>
                <td class="text-center" style="width: 50px;">
                    <button type="button" class="btn btn-sm btn-outline-danger remove-item-btn"><i class="bi bi-trash"></i></button>
                </td>
            `;
            itemsTableBody.appendChild(tr);
            attachRowEvents(tr);
            updateTotals();
        });

        function attachRowEvents(row) {
            row.querySelectorAll("input, select").forEach(input => {
                input.addEventListener("input", updateTotals);
                input.addEventListener("change", updateTotals);
            });
            const removeBtn = row.querySelector(".remove-item-btn");
            if (removeBtn) {
                removeBtn.addEventListener("click", function () {
                    if (itemsTableBody.querySelectorAll("tr").length > 1) {
                        row.remove();
                        updateTotals();
                    } else {
                        alert("At least one export item is required for customs declaration.");
                    }
                });
            }
        }

        itemsTableBody.querySelectorAll("tr").forEach(attachRowEvents);
        updateTotals();
    }

    // 6. Intersection Observer for Scroll-Triggered Reveal Animations
    const revealObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add("revealed");
                // Trigger count-up if element contains counter
                const counters = entry.target.querySelectorAll(".stat-counter");
                counters.forEach(counter => animateCounter(counter));
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.15 });

    document.querySelectorAll(".reveal-on-scroll").forEach(el => {
        revealObserver.observe(el);
    });

    // 7. Dynamic Number Count-Up Animation
    function animateCounter(el) {
        const target = parseFloat(el.getAttribute("data-target") || el.innerText.replace(/[^0-9.]/g, ""));
        const prefix = el.getAttribute("data-prefix") || "";
        const suffix = el.getAttribute("data-suffix") || "";
        const duration = 1600; // ms
        const startTime = performance.now();

        function updateCount(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            // Ease out cubic
            const easeProgress = 1 - Math.pow(1 - progress, 3);
            const currentVal = Math.floor(easeProgress * target);

            el.innerText = `${prefix}${currentVal.toLocaleString('en-IN')}${suffix}`;

            if (progress < 1) {
                requestAnimationFrame(updateCount);
            } else {
                el.innerText = `${prefix}${target.toLocaleString('en-IN')}${suffix}`;
            }
        }
        requestAnimationFrame(updateCount);
    }

    // Run counters on load for elements already in view
    document.querySelectorAll(".stat-counter").forEach(counter => {
        if (!counter.closest(".reveal-on-scroll")) {
            animateCounter(counter);
        }
    });

    // 8. 3D Card Interactive Tilt Effect
    document.querySelectorAll(".tilt-card").forEach(card => {
        card.addEventListener("mousemove", function (e) {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;
            const rotateX = (-y / (rect.height / 2)) * 6; // max 6 deg
            const rotateY = (x / (rect.width / 2)) * 6;
            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-8px)`;
        });

        card.addEventListener("mouseleave", function () {
            card.style.transform = "perspective(1000px) rotateX(0deg) rotateY(0deg) translateY(0)";
        });
    });

    // =========================================================================
    // 9. BILINGUAL LANGUAGE ENGINE & ENTRY MODAL (Hindi / English)
    // =========================================================================
    const dnkTranslations = {
        en: {
            demo_role: "Demo Role:",
            nav_exporter: "Exporter",
            nav_staff: "DNK Staff",
            nav_customs: "Customs",
            nav_admin: "Admin",
            nav_home: "Home",
            nav_book: "Book Export",
            nav_track: "Track Article",
            nav_calculator: "Postage Calculator",
            nav_locator: "Find Kendra",
            nav_exporter_desk: "Exporter Desk",
            nav_exporter_welcome: "Exporter Welcome Hub",
            nav_dashboard: "Dashboard",
            nav_book_pbe: "Book New e-PBE",
            nav_my_shipments: "My Shipments & Labels",
            nav_counter_portal: "Counter Portal",
            nav_customs_portal: "Customs Portal",
            nav_admin_portal: "Admin Portal",
            btn_book_epbe: "Book e-PBE",
            hero_ministry: "Department of Posts • Govt. of India",
            hero_title_1: "Export Worldwide from Your Local",
            hero_title_2: "Dak Ghar",
            hero_desc: "The Dak Ghar Niryat Kendra (DNK) platform enables Indian MSMEs, artisans, and e-commerce sellers to ship globally with 100% paperless e-PBE filing, zero-rated IGST, automated customs clearance, and doorstep pickup.",
            btn_book_export: "Book Export (e-PBE)",
            btn_track_article: "Track Article",
            badge_igst: "Zero-Rated IGST",
            badge_upu: "UPU S10 Barcode",
            badge_countries: "210+ Countries",
            badge_pickup: "Doorstep Pickup",
            tab_track: "Track Consignment",
            tab_rate: "Quick Rate Check",
            track_title: "Track Your Export Article",
            track_desc: "Enter your 13-digit UPU S10 Barcode tracking number.",
            btn_track: "Track",
            sample_consignments: "Try sample live consignments:",
            rate_title: "Postage & Transit Estimator",
            rate_desc: "Instant calculation for major international destinations.",
            dest_country: "Destination Country",
            weight_kg: "Weight (Kg)",
            ems_name: "EMS Speed Post",
            ems_transit: "3-7 Days",
            parcel_name: "Air Parcel",
            parcel_transit: "6-10 Days",
            btn_advanced_calc: "Open Advanced Tariff Calculator",
            stat_kendras: "DNK Export Kendras",
            stat_exporters: "Registered Exporters",
            stat_shipments: "Dispatched Shipments",
            stat_destinations: "Global Destinations",
            steps_badge: "Easy 3-Step Process",
            steps_title: "How to Export with Dak Ghar Niryat Kendra",
            steps_desc: "Exporting your goods internationally is straightforward with our digital postal system.",
            step1_title: "Book e-PBE Online",
            step1_desc: "Fill in recipient address, package weight, and goods description. The system generates your electronic Postal Bill of Export and printable S10 barcode label.",
            step2_title: "Drop Off or Request Pickup",
            step2_desc: "Hand over your parcel at any designated Dak Ghar Niryat Kendra counter, or schedule a doorstep pickup by your local postman.",
            step3_title: "Customs & Global Delivery",
            step3_desc: "Parcels are electronically cleared through Customs at Foreign Post Offices (FPO) and dispatched via international air mail to 210+ countries.",
            portals_title: "Dedicated Stakeholder Portals",
            portals_desc: "Direct access for exporters, postal staff, customs appraisers, and administrators",
            portal_exporter_title: "MSME Exporters",
            portal_exporter_badge: "Self Service",
            portal_exporter_desc: "Book e-PBE export shipments, print barcodes, view duty drawback, and manage bookings.",
            portal_exporter_btn: "Exporter Desk",
            portal_staff_title: "DNK Counter Staff",
            portal_staff_badge: "Postal Operations",
            portal_staff_desc: "Weigh parcels, scan barcodes at counter induction, and generate FPO dispatch bags.",
            portal_staff_btn: "Counter Portal",
            portal_customs_title: "Customs & FPO",
            portal_customs_badge: "Sec 84 Customs",
            portal_customs_desc: "Scrutinize electronic declarations, assess export duties, and grant Out of Charge (OOC).",
            portal_customs_btn: "Customs Portal",
            portal_admin_title: "Admin & Analytics",
            portal_admin_badge: "Directorate",
            portal_admin_desc: "National performance metrics, Kendra network oversight, MSME registry, and tariff controls.",
            portal_admin_btn: "Admin Portal",
            services_title: "Postal Export Services",
            services_desc: "Reliable, affordable shipping options through India Post",
            btn_view_tariff: "View Full Tariff Table",
            badge_fastest: "Fastest",
            ems_max_weight: "Up to 30 Kg",
            srv_ems_title: "International Speed Post (EMS)",
            srv_ems_desc: "Priority international air conveyance with end-to-end milestone tracking and expedited customs clearance.",
            lbl_transit_time: "Transit Time:",
            srv_ems_days: "3 - 7 Business Days",
            badge_bulk: "Bulk Friendly",
            parcel_max_weight: "Up to 20 Kg",
            srv_parcel_title: "International Air Parcel",
            srv_parcel_desc: "Cost-effective scheduled air freight solution for bulk commercial shipments, handicrafts, textiles, and merchandise.",
            srv_parcel_days: "6 - 12 Business Days",
            badge_ecommerce: "E-Commerce",
            itps_max_weight: "Up to 2.0 Kg",
            srv_itps_title: "ITPS (Tracked Packet)",
            srv_itps_desc: "Special low-cost tracked packet service designed for Amazon, Etsy, and eBay cross-border e-commerce sellers.",
            srv_itps_days: "5 - 10 Business Days",
            locator_banner_title: "Find Your Nearest Dak Ghar Niryat Kendra",
            locator_banner_desc: "Over 8+ major head post offices are currently operational with dedicated export desks, weighing scales, and barcode induction counters.",
            btn_locate_kendras: "Locate Kendras",
            pillars_badge: "Exporter Advantages",
            pillars_title: "Why Choose Dak Ghar Niryat Kendra?",
            pillars_desc: "Government of India backed incentives, streamlined customs corridors, and comprehensive postal reach.",
            feat1_title: "100% Paperless e-PBE",
            feat1_desc: "Direct electronic Postal Bill of Export filing integrated with ICEGATE Customs EDI gateway under Section 84 of Customs Act.",
            feat2_title: "Zero-Rated IGST (LUT RFD-11)",
            feat2_desc: "Export without upfront payment of Integrated GST by linking your active Letter of Undertaking (LUT) reference number.",
            feat3_title: "RoDTEP Export Rebates",
            feat3_desc: "Automatic duty drawback credit and RoDTEP claims calculated on FOB values for handicrafts, textiles, and merchandise.",
            feat4_title: "UPU S10 Standard Barcode",
            feat4_desc: "13-character Universal Postal Union barcode recognized across 192 postal administrations globally with milestone scans.",
            feat5_title: "Doorstep Postman Pickup",
            feat5_desc: "Schedule convenient export parcel collection by your local beat postman directly from your workshop or warehouse.",
            feat6_title: "210+ Global Destinations",
            feat6_desc: "Direct air cargo connectivity to North America, Europe, Middle East, Asia-Pacific, and Latin America.",
            stories_badge: "Success Stories",
            stories_title: "Empowering Indian Artisans Globally",
            stories_desc: "Hear how micro & small enterprises across India are expanding overseas through Dak Ghar Niryat Kendra.",
            faq_badge: "Help & Guidelines",
            faq_title: "Frequently Asked Questions",
            faq_desc: "Everything you need to know about exporting through the Dak Ghar Niryat Kendra portal.",
            faq1_q: "What documents do I need to register as an MSME Exporter on DNK?",
            faq1_a: "To register, you need: 1) Valid DGFT Import-Export Code (IEC), 2) GSTIN registration, 3) Active Letter of Undertaking (LUT RFD-11) for zero-rated IGST export, and 4) Authorized Dealer (AD) Code registered with Customs.",
            faq2_q: "What is an electronic Postal Bill of Export (e-PBE)?",
            faq2_a: "An e-PBE is a paperless commercial export declaration governed by Section 84 of the Customs Act, 1962. It replaces traditional manual shipping bills and transmits consignment data directly to the Foreign Post Office (FPO) customs system.",
            faq3_q: "How does Doorstep Pickup work for export parcels?",
            faq3_a: "Once your e-PBE is filed and S10 barcode printed, you can request pickup from your Exporter Desk. Your assigned beat postman receives the collection task and picks up the parcel directly from your premises with digital barcode scanning.",
            faq4_q: "Can I claim RoDTEP and Duty Drawback on postal exports?",
            faq4_a: "Yes. Consignments exported via e-PBE are fully eligible for RoDTEP (Remission of Duties and Taxes on Exported Products) and duty drawback benefits directly credited to your authorized bank account."
        },
        hi: {
            demo_role: "डेमो भूमिका:",
            nav_exporter: "निर्यातक",
            nav_staff: "डीएनके स्टाफ",
            nav_customs: "कस्टम्स",
            nav_admin: "एडमिन",
            nav_home: "होम / मुख्य पृष्ठ",
            nav_book: "निर्यात बुक करें",
            nav_track: "पार्सल ट्रैक करें",
            nav_calculator: "डाक शुल्क गणक",
            nav_locator: "केंद्र खोजें",
            nav_exporter_desk: "निर्यातक डेस्क",
            nav_exporter_welcome: "निर्यातक स्वागत हब",
            nav_dashboard: "डैशबोर्ड",
            nav_book_pbe: "नया e-PBE बुक करें",
            nav_my_shipments: "मेरे पार्सल और लेबल",
            nav_counter_portal: "काउंटर पोर्टल",
            nav_customs_portal: "कस्टम्स पोर्टल",
            nav_admin_portal: "एडमिन पोर्टल",
            btn_book_epbe: "e-PBE बुक करें",
            hero_ministry: "डाक विभाग • संचार मंत्रालय • भारत सरकार",
            hero_title_1: "अपने स्थानीय डाक घर से करें",
            hero_title_2: "वैश्विक निर्यात",
            hero_desc: "डाक घर निर्यात केंद्र (DNK) डिजिटल प्लेटफॉर्म भारतीय एमएसएमई, कारीगरों, बुनकरों और ई-कॉमर्स विक्रेताओं को 100% पेपरलेस e-PBE फाइलिंग, शून्य-रेटेड आईजीएसटी (LUT), स्वचालित सीमा शुल्क निकासी और डोरस्टेप पिकअप के साथ 210+ देशों में निर्यात करने की सुविधा प्रदान करता है।",
            btn_book_export: "निर्यात पार्सल बुक करें (e-PBE)",
            btn_track_article: "पार्सल ट्रैक करें",
            badge_igst: "शून्य आईजीएसटी (LUT)",
            badge_upu: "यूपीयू S10 बारकोड",
            badge_countries: "210+ गंतव्य देश",
            badge_pickup: "डोरस्टेप पोस्टमैन पिकअप",
            tab_track: "पार्सल ट्रैक करें",
            tab_rate: "त्वरित डाक शुल्क जांच",
            track_title: "अपना निर्यात पार्सल ट्रैक करें",
            track_desc: "अपना 13-अंकों का यूपीयू S10 बारकोड नंबर दर्ज करें।",
            btn_track: "ट्रैक करें",
            sample_consignments: "लाइव नमूना पार्सल जांचें:",
            rate_title: "डाक शुल्क एवं समय अनुमान",
            rate_desc: "प्रमुख अंतरराष्ट्रीय गंतव्यों के लिए तुरंत गणना करें।",
            dest_country: "गंतव्य देश",
            weight_kg: "वजन (किग्रा)",
            ems_name: "अंतरराष्ट्रीय स्पीड पोस्ट (EMS)",
            ems_transit: "3-7 कार्य दिवस",
            parcel_name: "एयर पार्सल",
            parcel_transit: "6-10 कार्य दिवस",
            btn_advanced_calc: "विस्तृत टैरिफ कैलकुलेटर खोलें",
            stat_kendras: "सक्रिय डीएनके केंद्र",
            stat_exporters: "पंजीकृत निर्यातक",
            stat_shipments: "प्रेषित निर्यात पार्सल",
            stat_destinations: "वैश्विक गंतव्य देश",
            steps_badge: "आसान 3-चरणीय प्रक्रिया",
            steps_title: "डाक घर निर्यात केंद्र से निर्यात कैसे करें",
            steps_desc: "हमारी डिजिटल डाक प्रणाली के साथ अपने सामान का अंतरराष्ट्रीय स्तर पर निर्यात करना बेहद सरल है।",
            step1_title: "1. ऑनलाइन e-PBE फाइल करें",
            step1_desc: "प्राप्तकर्ता का पता, पार्सल का वजन और माल का विवरण दर्ज करें। सिस्टम स्वचालित रूप से इलेक्ट्रॉनिक पोस्टल बिल ऑफ एक्सपोर्ट और S10 बारकोड लेबल उत्पन्न करता है।",
            step2_title: "2. केंद्र पर जमा करें या पिकअप चुनें",
            step2_desc: "अपने पार्सल को निकटतम डाक घर निर्यात केंद्र काउंटर पर सौंपें या अपने स्थानीय डाकिया द्वारा डोरस्टेप पिकअप का समय निर्धारित करें।",
            step3_title: "3. कस्टम्स क्लीयरेंस और वैश्विक डिलीवरी",
            step3_desc: "विदेश डाकघर (FPO) पर इलेक्ट्रॉनिक रूप से सीमा शुल्क क्लीयरेंस प्राप्त होने के बाद पार्सल अंतरराष्ट्रीय एयर कार्गो द्वारा 210+ देशों में वितरित किया जाता है।",
            portals_title: "समर्पित हितधारक पोर्टल",
            portals_desc: "निर्यातकों, डाक कर्मचारियों, कस्टम्स अधिकारियों और प्रशासकों के लिए सीधी पहुंच",
            portal_exporter_title: "एमएसएमई निर्यातक",
            portal_exporter_badge: "स्वयं सेवा",
            portal_exporter_desc: "e-PBE निर्यात शिपमेंट बुक करें, बारकोड प्रिंट करें, शुल्क छूट देखें और बुकिंग प्रबंधित करें।",
            portal_exporter_btn: "निर्यातक डेस्क",
            portal_staff_title: "डीएनके काउंटर स्टाफ",
            portal_staff_badge: "डाक परिचालन",
            portal_staff_desc: "पार्सल का वजन करें, काउंटर इंडक्शन पर बारकोड स्कैन करें और FPO डिस्पैच बैग तैयार करें।",
            portal_staff_btn: "काउंटर पोर्टल",
            portal_customs_title: "कस्टम्स और विदेश डाकघर (FPO)",
            portal_customs_badge: "धारा 84 सीमा शुल्क",
            portal_customs_desc: "इलेक्ट्रॉनिक घोषणाओं की जांच करें, निर्यात शुल्क का आकलन करें और आउट ऑफ चार्ज (OOC) जारी करें।",
            portal_customs_btn: "कस्टम्स पोर्टल",
            portal_admin_title: "प्रशासन और एनालिटिक्स",
            portal_admin_badge: "निदेशालय",
            portal_admin_desc: "राष्ट्रीय प्रदर्शन मेट्रिक्स, केंद्र नेटवर्क प्रबंधन, एमएसएमई निर्देशिका और टैरिफ नियंत्रण।",
            portal_admin_btn: "एडमिन पोर्टल",
            services_title: "अंतरराष्ट्रीय डाक निर्यात सेवाएं",
            services_desc: "इंडिया पोस्ट के माध्यम से विश्वसनीय और किफायती अंतरराष्ट्रीय शिपिंग",
            btn_view_tariff: "पूरा टैरिफ चार्ट देखें",
            badge_fastest: "सबसे तेज़",
            ems_max_weight: "30 किग्रा तक",
            srv_ems_title: "अंतरराष्ट्रीय स्पीड पोस्ट (EMS)",
            srv_ems_desc: "प्राथमिकता एयर मेल, एंड-टू-एंड लाइव ट्रैकिंग और त्वरित कस्टम्स कॉरिडोर सुविधा।",
            lbl_transit_time: "डिलीवरी समय:",
            srv_ems_days: "3 - 7 कार्य दिवस",
            badge_bulk: "थोक निर्यात हेतु",
            parcel_max_weight: "20 किग्रा तक",
            srv_parcel_title: "अंतरराष्ट्रीय एयर पार्सल",
            srv_parcel_desc: "थोक वाणिज्यिक शिपमेंट, हस्तशिल्प, वस्त्र और माल के लिए किफायती अनुसूचित एयर फ्रेट समाधान।",
            srv_parcel_days: "6 - 12 कार्य दिवस",
            badge_ecommerce: "ई-कॉमर्स स्पेशल",
            itps_max_weight: "2.0 किग्रा तक",
            srv_itps_title: "आईटीपीएस (ट्रैक्ड पैकेट)",
            srv_itps_desc: "अमेज़न, एटीसी और विदेशी ई-कॉमर्स विक्रेताओं के लिए विशेष कम लागत वाली ट्रैक्ड पैकेट सेवा।",
            srv_itps_days: "5 - 10 कार्य दिवस",
            locator_banner_title: "अपना निकटतम डाक घर निर्यात केंद्र खोजें",
            locator_banner_desc: "समर्पित निर्यात डेस्क, इलेक्ट्रॉनिक वजन कांटे और बारकोड इंडक्शन से लैस 8+ प्रधान डाकघर सक्रिय हैं।",
            btn_locate_kendras: "केंद्र खोजें",
            pillars_badge: "निर्यातक लाभ",
            pillars_title: "डाक घर निर्यात केंद्र क्यों चुनें?",
            pillars_desc: "भारत सरकार द्वारा समर्थित प्रोत्साहन, सुव्यवस्थित कस्टम्स कॉरिडोर और व्यापक डाक पहुंच।",
            feat1_title: "100% पेपरलेस e-PBE",
            feat1_desc: "कस्टम्स अधिनियम की धारा 84 के तहत ICEGATE कस्टम्स EDI गेटवे के साथ एकीकृत प्रत्यक्ष इलेक्ट्रॉनिक डाक निर्यात बिल।",
            feat2_title: "शून्य आईजीएसटी (LUT RFD-11)",
            feat2_desc: "अपने सक्रिय लेटर ऑफ अंडरटेकिंग (LUT) को जोड़कर बिना अग्रिम आईजीएसटी भुगतान के निर्यात करें।",
            feat3_title: "RoDTEP निर्यात छूट",
            feat3_desc: "हस्तशिल्प, वस्त्र और माल के एफओबी मूल्य पर स्वचालित ड्यूटी ड्रॉबैक और RoDTEP लाभ प्राप्त करें।",
            feat4_title: "यूपीयू S10 मानक बारकोड",
            feat4_desc: "192 वैश्विक डाक प्रशासनों द्वारा मान्यता प्राप्त 13-अंकों का यूनिवर्सल पोस्टल यूनियन बारकोड।",
            feat5_title: "डोरस्टेप डाकिया पिकअप",
            feat5_desc: "अपनी कार्यशाला या गोदाम से सीधे अपने स्थानीय डाकिए द्वारा निर्यात पार्सल संग्रह का समय निर्धारित करें।",
            feat6_title: "210+ वैश्विक गंतव्य",
            feat6_desc: "उत्तरी अमेरिका, यूरोप, मध्य पूर्व, एशिया-प्रशांत और लैटिन अमेरिका के लिए सीधा एयर कार्गो नेटवर्क।",
            stories_badge: "सफलता की कहानियां",
            stories_title: "भारतीय कारीगरों का वैश्विक सशक्तिकरण",
            stories_desc: "जानें कि कैसे भारत भर के सूक्ष्म एवं लघु उद्यमी डाक घर निर्यात केंद्र के माध्यम से विदेश में विस्तार कर रहे हैं।",
            faq_badge: "सहायता एवं दिशानिर्देश",
            faq_title: "अक्सर पूछे जाने वाले प्रश्न (FAQ)",
            faq_desc: "डाक घर निर्यात केंद्र पोर्टल के माध्यम से निर्यात करने के बारे में आपको जो कुछ जानने की आवश्यकता है।",
            faq1_q: "DNK पर MSME निर्यातक के रूप में पंजीकरण करने के लिए किन दस्तावेजों की आवश्यकता है?",
            faq1_a: "पंजीकरण के लिए आपको चाहिए: 1) वैध DGFT आयात-निर्यात कोड (IEC), 2) GSTIN पंजीकरण, 3) शून्य-रेटेड निर्यात के लिए सक्रिय LUT RFD-11, और 4) सीमा शुल्क के साथ पंजीकृत एडी कोड।",
            faq2_q: "इलेक्ट्रॉनिक पोस्टल बिल ऑफ एक्सपोर्ट (e-PBE) क्या है?",
            faq2_a: "e-PBE सीमा शुल्क अधिनियम, 1962 की धारा 84 द्वारा शासित एक पेपरलेस निर्यात घोषणा है। यह विदेश डाकघर (FPO) सीमा शुल्क प्रणाली को सीधे डेटा भेजता है।",
            faq3_q: "निर्यात पार्सल के लिए डोरस्टेप पिकअप कैसे काम करता है?",
            faq3_a: "e-PBE फाइल करने और S10 बारकोड प्रिंट करने के बाद, आप अपने निर्यातक डेस्क से पिकअप का अनुरोध कर सकते हैं। आपका स्थानीय डाकिया आपके पते पर आकर बारकोड स्कैन करके पार्सल लेता है।",
            faq4_q: "क्या मैं डाक निर्यात पर RoDTEP और ड्यूटी ड्रॉबैक का दावा कर सकता हूं?",
            faq4_a: "हाँ। e-PBE के माध्यम से निर्यात किए गए पार्सल RoDTEP और ड्यूटी ड्रॉबैक लाभों के लिए पूरी तरह से पात्र हैं, जो सीधे आपके बैंक खाते में जमा होते हैं।"
        }
    };

    window.selectLanguage = function (lang) {
        localStorage.setItem("dnk_preferred_language", lang);
        applyLanguage(lang);

        // Close modal
        const modalEl = document.getElementById("languageSelectModal");
        if (modalEl) {
            const modalInstance = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
            modalInstance.hide();
        }
    };

    window.openLanguageModal = function () {
        const modalEl = document.getElementById("languageSelectModal");
        if (modalEl) {
            const curLang = localStorage.getItem("dnk_preferred_language") || "en";
            document.querySelectorAll(".lang-modal-card").forEach(c => c.classList.remove("active"));
            const activeCard = document.getElementById(curLang === "hi" ? "btnLangHi" : "btnLangEn");
            if (activeCard) activeCard.classList.add("active");

            const modalInstance = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
            modalInstance.show();
        }
    };

    window.dismissLanguageModal = function () {
        if (!localStorage.getItem("dnk_preferred_language")) {
            localStorage.setItem("dnk_preferred_language", "en");
            applyLanguage("en");
        }
    };

    function applyLanguage(lang) {
        const dict = dnkTranslations[lang] || dnkTranslations.en;
        document.querySelectorAll("[data-i18n]").forEach(el => {
            const key = el.getAttribute("data-i18n");
            if (dict[key]) {
                el.innerHTML = dict[key];
            }
        });

        // Update active badge in top navbar
        const badge = document.getElementById("activeLangBadge");
        if (badge) {
            badge.innerHTML = lang === "hi" ? "🇮🇳 हिन्दी" : "🌐 English";
        }

        // Set html lang attribute
        document.documentElement.lang = lang;
    }

    // Auto-check language preference on entry
    const savedLang = localStorage.getItem("dnk_preferred_language");
    if (savedLang) {
        applyLanguage(savedLang);
    } else {
        // Show language selection modal on first visit
        setTimeout(() => {
            window.openLanguageModal();
        }, 300);
    }
});



// Session 3: قواعد Validation إضافية في المتصفح
// لازم الملف ده يتحمّل بعد jquery.validate.unobtrusive وقبل ما الصفحة تخلص تحميل
// (بيتحمّل من _ValidationScriptsPartial.cshtml)

// (1) Custom rule: رقم موبايل مصري — نفس Regex بتاع EgyptianPhoneAttribute في السيرفر
// addMethod(اسم الـ rule, function بترجع true لو القيمة صح)
$.validator.addMethod("egphone", function (value, element) {
    if (this.optional(element)) return true;        // الحقل فاضي → دي مسؤولية required مش إحنا
    return /^01[0125]\d{8}$/.test(value.replace(/[\s-]/g, ""));   // نشيل المسافات والشَرط زي السيرفر
});

// Adapter: بيربط الـ attribute اللي في الـ HTML (data-val-egphone="...") بالـ rule اللي اسمها egphone
// addBool = الـ rule مالهاش parameters — والرسالة بتتاخد من قيمة الـ attribute نفسه
$.validator.unobtrusive.adapters.addBool("egphone");

// (2) خلّي number/range يقبلوا الأرقام العربي "١٢٠" زي الـ FlexibleDecimalModelBinder في السيرفر
// toLatin: بيشيل "EGP / ج.م / جنيه" ويحوّل ٠-٩ (عربي) و ۰-۹ (فارسي) لـ 0-9
const toLatin = v => (v ?? "").toString()
    .replace(/EGP|LE|ج\.م|جنيه/gi, "")
    .replace(/[٠-٩]/g, d => d.charCodeAt(0) - 0x0660)
    .replace(/[۰-۹]/g, d => d.charCodeAt(0) - 0x06F0)
    .replace(/٫/g, ".")                // الفاصلة العشرية العربي
    .replace(/[,٬\s]/g, "");           // فواصل الآلاف والمسافات

// بنلف (wrap) الـ methods الأصلية: نحوّل القيمة للاتيني الأول وبعدين نسيب الأصلية تحكم
["number", "range", "min", "max"].forEach(name => {
    const original = $.validator.methods[name];
    $.validator.methods[name] = function (value, element, param) {
        return original.call(this, toLatin(value), element, param);
    };
});



setTimeout(function () {
    check();
}, 3000);

function check() {
    const pathname = window.location.pathname;

    var xhr = null;
    try {
        xhr = new XMLHttpRequest();
    } catch (error) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    }
    //2、调用open
    xhr.open("get", "/home/GetbuttonPermissionList?pathname=" + pathname, true);
    //3、调用send
    xhr.send();
    //4、等待数据响应
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            //判断本次下载的状态码都是多少
            if (xhr.status == 200) {
                //alert(xhr.responseText);
                //console.log(xhr);
                //console.log(xhr.responseText);
                let obj = JSON.parse(xhr.responseText);
                if (obj == null || obj == undefined) {
                    return;
                }
                if (obj.length == 0) {
                    return;
                }
                console.log(obj);
                let eleType = "";
                for (const item of obj) {
                    //console.log(item.ButtonID, item.Permission);
                    //document.getElementById(item.ButtonID).disabled = item.Permission;
                    //document.getElementById(item.ButtonID).style.display = item.Permission ? "block" : "none";
                    let ele = document.getElementById(item.ButtonID);
                    if (ele != null && ele != undefined) {

                        if (ele.nodeName == "INPUT") {
                            console.log('属性存在INPUT');
                            if (ele.hasAttribute('type')) {
                                //console.log('属性存在');
                                let attributeValue = ele.getAttribute('type');
                                if (attributeValue == "button") {
                                    eleType = "button";
                                }
                                if (attributeValue == "text") {
                                    eleType = "text";
                                }
                            }
                        }
                        if (ele.nodeName == "BUTTON") {
                            console.log('属性存在BUTTON');
                            eleType = "button";
                        }
                        if (ele.nodeName == "A") {
                            console.log('属性存在A');
                            eleType = "A";
                        }
                        
                        //let display = document.getElementById(item.ButtonID).style.display;
                        //document.getElementById(item.ButtonID).style.display = item.Permission ? display: "none";// 隐藏不可见
                        //document.getElementById(item.ButtonID).disabled = item.Permission;// 禁用

                        //let display = ele.style.display;
                        //ele..style.display = item.Permission ? display: "none";// 隐藏不可见
                        if (eleType == "button" || eleType == "text") {
                            ele.disabled = item.Permission;// 禁用
                        }
                        if (eleType == "A") {
                            ele.href = "";
                          //let display = ele.style.display;
                          //ele.style.display = item.Permission ? display: "none";// 隐藏不可见
                            //ele.onclick = function (e) {
                            //    alert('链接地址是：' + this.href + ', 已禁止跳转');
                            //    stopDefault(e);
                            //}
                            //let redirected = false;
                            ele.addEventListener('click', ev => {
                                alert('链接地址已禁止跳转');
                                ev.preventDefault();
                                //if (redirected) {
                                //    ev.preventDefault();
                                //} else {
                                //    console.log('click');
                                //    redirected = true;
                                //    link.style.cursor = 'wait';
                                //}
                            });
                        }
                    }
                }

            } else {
                alert("Error:" + pathname + " = "+ xhr.status);
            }
        }
    }

}
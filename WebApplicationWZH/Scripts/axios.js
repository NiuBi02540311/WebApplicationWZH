
  //https://www.szhulian.com/new/518.html


 
// target 是DOM里已经存在的元素
// aim 是要插入的新元素
function insertAfter(target, aim) {
    target.nextSibling ? target.parentNode.insertBefore(aim, target.nextSibling) : target.parentNode.appendChild(aim);
}


function myFunction() {
    var node = document.createElement("LI");
    var textnode = document.createTextNode("Water");
    node.appendChild(textnode);
    document.getElementById("myList").appendChild(node);
}

var index = 1;
function createElement() {
    // 创建一个新的div元素
    var newDiv = document.createElement("div");

    // 设置新元素的属性或样式
    //newDiv.id = "myNewDiv";
    //newDiv.style.backgroundColor = "red";
    newDiv.innerHTML = "<strong>警告！</strong>数据加载中-"  + index;
    newDiv.className ="alert alert-danger"
    // 将新元素添加到页面上的指定位置（这里为body）()
    //1.appendChild() 方法可向节点的子节点列表的末尾添加新的子节点。
    //document.createElement()是在对象中创建一个对象，要与appendChild() 或 insertBefore()方法联合使用。
    //其中，appendChild() 方法在节点的子节点列表末添加新的子节点。insertBefore() 方法在节点的子节点列表任意位置插入新的节点

    //document.body.appendChild(newDiv);
    //var firstNode = document.querySelector(); // 第一个元素
    //let firstNode = document.getElementsByTagName('body').childNodes[0];


    //var parentElement = document.getElementsByTagName('body');
    //var firstNode2 = parentElement.firstElementChild;
    var firstNode = document.body.firstChild;
    //let nodes = document.querySelectorAll('body');

    
    //console.log(firstNode2)
    $divList.push(newDiv);
    document.body.insertBefore(newDiv, firstNode);
    index++;
}

/*
 const cssCode = `
    .myClass {
        color: red;
        font-size: 14px;
    }
`;

const styleTag = document.createElement('style');
styleTag.innerHTML = cssCode;
document.head.appendChild(styleTag);
 */


 //httpss://blog.csdn.net/kuang_nu/article/details/130927143
const css = '.my-class { color: red; display:block ;border: 1px solid red ;background-color:yellow;margin-top:5px;}';
const $style = document.createElement('style');
$style.type = 'text/css';
$style.textContent = css;
document.head.appendChild($style);





function dynamicImportJS(url) {
    var script = document.createElement('script'); // 创建一个新的script元素节点
    script.type = 'text/javascript'; // 指定脚本类型为JavaScript
    script.src = url; // 设置要引入的js文件路径

    if (typeof window !== "undefined" && typeof document !== "undefined") {
        // 如果当前运行环境是浏览器端，则将script节点添加到head部分
        document.getElementsByTagName("body")[0].appendChild(script);
    } else {
        // 否则，将script节点添加到全局对象（Node.js）的global变量上
        global.document.getElementsByTagName("body")[0].appendChild(script);
    }
}

// 调用dynamicImportJS()函数来动态引入js文件
//dynamicImportJS('https://unpkg.com/axios@1.6.7/dist/axios.min.js');


//httpss://cloud.tencent.com/developer/article/1098141
const _axios = axios.create({
    baseURL: 'http://localhost:57526',
    timeout: 100000,
    headers: { 'X-Custom-Header': 'foobar' },
    withCredentials: false, // 默认的  表示跨域请求时是否需要使用凭证
});

// 在实例已创建后修改默认值
//_axios.defaults.headers.common["Authorization"] = AUTH_TOKEN;

var $divList = [];
// 添加请求拦截器
_axios.interceptors.request.use(function (config) {
    // 在发送请求之前做些什么
    //var node = document.querySelector("div");
    //node.innerHTML = "数据加载中"
    //node.className += "my-class";
    //node.style.backgroundColor = "blue"
    //node.addClass("my-class");

    // 移除类名
    //element.className = element.className.replace(/(?:^|\s)myClass(?!\S)/g, ''); // 使用正则表达式去除指定的类名
    $div = createElement();
    return config;
}, function (error) {
    // 对请求错误做些什么
    return Promise.reject(error);
});

// 添加响应拦截器
_axios.interceptors.response.use(function (response) {
    // 对响应数据做点什么
    //var node = document.querySelector("div");
    //node.innerHTML = "";
    console.log("interceptors.response");
    console.log(response);
    const config = response.config
    //return _axios(config) 重新发起请求
    HideLoading();
    return response;
}, function (error) {
    // 对响应错误做点什么
    HideLoading();
    return Promise.reject(error);
});

function HideLoading() {
    if ($divList && $divList != null && $divList.length > 0) {
        setTimeout(function () {
            //document.body.removeChild($div)
            let item = $divList.shift();
            item.remove();
        }, 5000);
    }
}
var Test = {
    createDiv: function () {
        var div = document.createElement('div');
        div.id = "createDiv";
        div.style.cssText = 'border:1px solid red; width:200px; z-index:100; height:20px;';
        document.body.appendChild(div);
    },
    appendDivChild: function () {
        var div = document.createElement('div');
        div.id = "appendDivChild";
        div.style.cssText = 'border:1px solid green; width:400px; z-index:100; height:100px;';
        var childDiv = document.createElement('div');
        childDiv.id = "childDiv";
        childDiv.style.cssText = 'border:2px solid gray; width:200px; z-index:100; height:50px;';
        div.appendChild(childDiv);
        document.body.appendChild(div);
    },
    createSelect: function () {
        var select = document.createElement('select');
        select.id = "select";
        var option1 = document.createElement('option');
        option1.value = 1;
        option1.text = 1;//非ie,添加内容
        option1.innerHTML = 1;//ie,添加内容
        select.appendChild(option1);
        var option2 = document.createElement('option');
        option2.value = 2;
        option2.text = 2;
        option2.innerHTML = 2;
        select.appendChild(option2);
        var option3 = document.createElement('option');
        option3.value = 3;
        option3.text = 1;
        option3.innerHTML = 3;
        select.appendChild(option3);
        document.body.appendChild(select);
    },
    createRadio: function () {
        var radio = document.createElement('input');
        radio.id = 'radio';
        radio.type = "radio";
        radio.width = "100";
        var label = document.createElement('label');
        label.text = "男";
        label.innerHTML = "男";
        document.body.appendChild(radio);
        document.body.appendChild(label);
    }
};
using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnet02_boardapp.Controllers
{
    // https://localhost:7800/Board/Index 
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db;   // db 알아서 연결
        }
        public IActionResult Index()    // 게시판 최초화면 리스트
        {
            IEnumerable<Board> objBoardList = _db.Boards.ToList();  // SELECT 쿼리
            return View(objBoardList);
        }

        // https://localhost:7059/Board/Create
        // GetMethod로 화면 URL로 부를때 액션

        public IActionResult Create()   // 게시판 글쓰기
        {
            return View();
        }

        // Submit이 발생해서 내부로 데이터를 전달 액션

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Board board)
        {
            _db.Boards.Add(board);  //INSERT
            _db.SaveChanges();  // COMMIT

            return RedirectToAction("Index", "Board");
        }
    }
}

namespace SD_330_W22SD_Assignment.Models
{
    public class Pager
    {
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public Pager()
        {

        }

        public Pager(int total, int page, int pageSize = 10)
        {
            int currentPage = page;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;
            int totalPages = (int)Math.Ceiling((decimal)total / (decimal)pageSize);
            
            if(startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if(endPage > totalPages)
            {
                endPage = totalPages;
                if(endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            Total = total;
            CurrentPage = startPage;
            PageCount = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }
    }
}

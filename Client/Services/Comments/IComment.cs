using Core;

namespace Client
{
    public interface IComment
    {
        Task AddComment(NewComment comment, BrugerLoginDTO currentUser);
        Task DeleteComment();
    }
}
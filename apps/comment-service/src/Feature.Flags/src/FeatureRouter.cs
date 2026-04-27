using models;

namespace Feature.Flags;

public class FeatureRouter
{
    private readonly CreateCommentWithProfanity _withProfanity;
    private readonly CreateCommentWithoutProfanity _withoutProfanity;

    public FeatureRouter(
        CreateCommentWithProfanity withProfanity,
        CreateCommentWithoutProfanity withoutProfanity)
    {
        _withProfanity = withProfanity;
        _withoutProfanity = withoutProfanity;
    }

    public Task<CommentDto> CreateComment(CreateCommentDto dto,ConfigProfanity flags)
    {
        if (flags.IsProfanityEnabled)
            return _withProfanity.CreateAsync(dto);

        return _withoutProfanity.CreateAsync(dto);
    }
}